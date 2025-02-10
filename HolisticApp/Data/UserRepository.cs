using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using MySqlConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HolisticApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly string _connectionString;

        private const string selectAllUsersSql = "SELECT * FROM Users";
        private const string selectUserByIdSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Id = @id";
        private const string updateUserSql = @"
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
        private const string insertUserSql = @"
            INSERT INTO Users (Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, Role)
            VALUES (@username, @email, @passwordHash, @currentComplaint, @age, @gender, @height, @weight, @role)";
        private const string deleteUserSql = "DELETE FROM Users WHERE Id = @id";

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<MySqlConnection> GetConnectionAsync()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                _logger.LogDebug("Öffne Datenbankverbindung...");
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

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = connection.CreateCommand();
                command.CommandText = selectAllUsersSql;

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(CreateUserFromReader(reader));
                }
                _logger.LogInformation("Erfolgreich {UserCount} Benutzer aus der Datenbank geladen.", users.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Benutzerliste.");
            }
            return users;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = connection.CreateCommand();
                command.CommandText = selectUserByIdSql;
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    _logger.LogInformation("Benutzer mit ID {UserId} erfolgreich abgerufen.", id);
                    return CreateUserFromReader(reader);
                }
                _logger.LogWarning("Kein Benutzer mit ID {UserId} gefunden.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen des Benutzers mit ID {UserId}.", id);
            }
            return null;
        }

        public async Task<int> SaveUserAsync(User user)
        {
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = connection.CreateCommand();
                if (user.Id != 0)
                {
                    command.CommandText = updateUserSql;
                    command.Parameters.AddWithValue("@id", user.Id);
                }
                else
                {
                    command.CommandText = insertUserSql;
                }

                AddUserParameters(command, user);
                int result = await command.ExecuteNonQueryAsync();

                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) erfolgreich gespeichert.", user.Id);
                }
                else
                {
                    _logger.LogWarning("Speicherung des Benutzers (ID: {UserId}) fehlgeschlagen.", user.Id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Speichern des Benutzers (ID: {UserId}).", user.Id);
                return 0;
            }
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            try
            {
                using var connection = await GetConnectionAsync();
                using var command = connection.CreateCommand();
                command.CommandText = deleteUserSql;
                command.Parameters.AddWithValue("@id", id);

                int result = await command.ExecuteNonQueryAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) erfolgreich gelöscht.", id);
                }
                else
                {
                    _logger.LogWarning("Kein Benutzer mit ID {UserId} zum Löschen gefunden.", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Benutzers (ID: {UserId}).", id);
                return 0;
            }
        }

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

        private void AddUserParameters(MySqlCommand command, User user)
        {
            try
            {
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint ?? "Keine Beschwerden");
                command.Parameters.AddWithValue("@age", user.Age ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@height", user.Height ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@weight", user.Weight ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@role", user.Role.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen von Benutzerparametern zum SQL-Befehl.");
                throw;
            }
        }

        private string GetString(MySqlDataReader reader, string columnName, string defaultValue = "")
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen des Strings für die Spalte {ColumnName}.", columnName);
                return defaultValue;
            }
        }

        private int? GetNullableInt(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private decimal? GetNullableDecimal(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        private int GetInt(MySqlDataReader reader, string columnName, int defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }
    }
}