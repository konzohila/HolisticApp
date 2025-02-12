using System.Data;
using System.Security.Cryptography;
using System.Text;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace HolisticApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        // SQL-Abfragen als Konstanten
        private const string SelectUserByIdSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Id = @id";

        private const string SelectUserByEmailOrUsernameSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Email = @value OR Username = @value";

        private const string InsertUserSql = @"
            INSERT INTO Users 
            (Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, Role)
            VALUES (@username, @email, @passwordHash, @currentComplaint, @age, @gender, @height, @weight, @role)";

        private const string UpdateUserSql = @"
            UPDATE Users
            SET Username = @username, 
                Email = @email, 
                PasswordHash = @passwordHash,
                CurrentComplaint = @currentComplaint,
                Age = @age,
                Gender = @gender,
                Height = @height,
                Weight = @weight,
                Role = @role
            WHERE Id = @id";

        private const string DeleteUserSql = "DELETE FROM Users WHERE Id = @id";

        private const string CountUserByEmailOrUsernameSql = @"
            SELECT COUNT(*) 
            FROM Users 
            WHERE Email = @value OR Username = @value";

        private const string SelectUsersByRoleSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Role = @role";

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Erstellt und öffnet eine MySQL-Verbindung.
        /// </summary>
        private async Task<MySqlConnection> GetConnectionAsync()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.LogDebug("Datenbankverbindung erfolgreich geöffnet.");
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen der Datenbankverbindung.");
                throw;
            }
        }

        /// <summary>
        /// Berechnet den SHA256-Hash eines gegebenen Strings.
        /// </summary>
        private string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }

        /// <summary>
        /// Liest einen User aus dem aktuellen Datenleser aus.
        /// </summary>
        private User CreateUserFromReader(MySqlDataReader reader)
        {
            try
            {
                return new User
                {
                    Id = GetInt(reader, "Id"),
                    Username = GetString(reader, "Username"),
                    Email = GetString(reader, "Email"),
                    PasswordHash = GetString(reader, "PasswordHash"),
                    CurrentComplaint = GetString(reader, "CurrentComplaint", "Keine Beschwerden"),
                    Age = GetNullableInt(reader, "Age"),
                    Gender = GetString(reader, "Gender", "Nicht angegeben"),
                    Height = GetNullableDecimal(reader, "Height"),
                    Weight = GetNullableDecimal(reader, "Weight"),
                    MasterAccountId = GetNullableInt(reader, "MasterAccountId"),
                    Role = Enum.TryParse(GetString(reader, "Role", "Patient"), out UserRole parsedRole)
                        ? parsedRole
                        : UserRole.Patient
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen eines Benutzerobjekts aus dem Reader.");
                throw;
            }
        }

        /// <summary>
        /// Fügt alle Benutzerparameter dem SQL-Befehl hinzu.
        /// </summary>
        private void AddUserParameters(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint);
            command.Parameters.AddWithValue("@age", user.Age.HasValue ? (object)user.Age.Value : DBNull.Value);
            command.Parameters.AddWithValue("@gender", user.Gender);
            command.Parameters.AddWithValue("@height", user.Height.HasValue ? (object)user.Height.Value : DBNull.Value);
            command.Parameters.AddWithValue("@weight", user.Weight.HasValue ? (object)user.Weight.Value : DBNull.Value);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
        }

        /// <summary>
        /// Hilfsmethode, um einen String-Wert aus dem Reader auszulesen.
        /// </summary>
        private string GetString(MySqlDataReader reader, string columnName, string defaultValue = "")
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
        }

        private static int GetInt(MySqlDataReader reader, string columnName, int defaultValue = 0)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        private static int? GetNullableInt(MySqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private decimal? GetNullableDecimal(MySqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Speichert (registriert) einen neuen Benutzer in der Datenbank.
        /// </summary>
        public async Task<bool> SaveUserAsync(string username, string email, string password)
        {
            try
            {
                // Erstelle ein neues User-Objekt mit Standardwerten.
                var newUser = new User
                {
                    // Id wird von der DB generiert
                    Username = username,
                    Email = email,
                    PasswordHash = ComputeHash(password),
                    CurrentComplaint = "Keine Beschwerden",
                    Age = null,
                    Gender = "Nicht angegeben",
                    Height = null,
                    Weight = null,
                    Role = UserRole.Patient
                };

                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = InsertUserSql;
                AddUserParameters(command, newUser);

                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Neuer Benutzer {Email} erfolgreich registriert.", email);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Registrierung des Benutzers {Email} fehlgeschlagen.", email);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Speichern des Benutzers {Email}.", email);
                return false;
            }
        }

        /// <summary>
        /// Aktualisiert die Daten eines bestehenden Benutzers.
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("UpdateUserAsync wurde mit null übergeben.");
                return false;
            }

            try
            {
                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = UpdateUserSql;
                command.Parameters.AddWithValue("@id", user.Id);
                AddUserParameters(command, user);

                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) erfolgreich aktualisiert.", user.Id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Aktualisierung des Benutzers (ID: {UserId}) fehlgeschlagen.", user.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Benutzers (ID: {UserId}).", user.Id);
                return false;
            }
        }

        /// <summary>
        /// Löscht einen Benutzer anhand der ID.
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = DeleteUserSql;
                command.Parameters.AddWithValue("@id", id);

                var result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) erfolgreich gelöscht.", id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Kein Benutzer (ID: {UserId}) gefunden, um gelöscht zu werden.", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Benutzers (ID: {UserId}).", id);
                return false;
            }
        }

        /// <summary>
        /// Überprüft, ob bereits ein Benutzer mit der angegebenen E-Mail oder dem Benutzernamen existiert.
        /// </summary>
        public async Task<bool> IsUserInDatabaseAsync(string emailOrUsername)
        {
            try
            {
                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = CountUserByEmailOrUsernameSql;
                command.Parameters.AddWithValue("@value", emailOrUsername);

                var resultObj = await command.ExecuteScalarAsync();
                if (resultObj != null && int.TryParse(resultObj.ToString(), out int count))
                {
                    return count > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Überprüfen, ob der Benutzer {Value} bereits existiert.", emailOrUsername);
                return false;
            }
        }

        /// <summary>
        /// Sucht alle Benutzer mit der angegebenen Rolle.
        /// </summary>
        public async Task<List<User>> FindUsersByRole(UserRole role)
        {
            var users = new List<User>();

            try
            {
                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = SelectUsersByRoleSql;
                command.Parameters.AddWithValue("@role", role.ToString());

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(CreateUserFromReader(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen von Benutzern mit der Rolle {Role}.", role);
            }

            return users;
        }

        /// <summary>
        /// Authentifiziert einen Benutzer anhand von E-Mail/Username und Passwort.
        /// </summary>
        public async Task<AuthenticateResult> AuthenticateUser(string emailOrUsername, string password)
        {
            try
            {
                await using var connection = await GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = SelectUserByEmailOrUsernameSql;
                command.Parameters.AddWithValue("@value", emailOrUsername);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = CreateUserFromReader(reader);
                    var inputHash = ComputeHash(password);

                    if (string.Equals(user.PasswordHash, inputHash, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Benutzer {EmailOrUsername} erfolgreich authentifiziert.", emailOrUsername);
                        return new AuthenticateResult(user);
                    }
                    else
                    {
                        _logger.LogWarning("Authentifizierung fehlgeschlagen: Falsches Passwort für {EmailOrUsername}.", emailOrUsername);
                    }
                }
                else
                {
                    _logger.LogWarning("Authentifizierung fehlgeschlagen: Kein Benutzer für {EmailOrUsername} gefunden.", emailOrUsername);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der Authentifizierung für {EmailOrUsername}.", emailOrUsername);
            }
            return default; // IsAuthenticated = false, User = null
        }
    }
}