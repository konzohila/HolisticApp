using HolisticApp.Constants;
using HolisticApp.Models;
using HolisticApp.Helpers;
using HolisticApp.Mappers;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DbHelper _dbHelper;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbHelper = new DbHelper(connectionString, logger);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                await using var connection = await _dbHelper.GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return UserMapper.CreateUserFromReader(reader, _logger);
                }
                _logger.LogWarning("Kein Benutzer mit ID {UserId} gefunden.", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Benutzers mit ID {UserId}.", id);
                return null;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            var users = new List<User>();
            try
            {
                await using var connection = await _dbHelper.GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = SqlCommands.SelectUsersByRoleSql;
                command.Parameters.AddWithValue("@role", role.ToString());

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(UserMapper.CreateUserFromReader(reader, _logger));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen von Benutzern mit der Rolle {Role}.", role);
            }
            return users;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                int result = await _dbHelper.ExecuteNonQueryAsync(SqlCommands.InsertUserSql, cmd =>
                {
                    UserMapper.AddUserParameters(cmd, user);
                });
                if (result > 0)
                {
                    _logger.LogInformation("Neuer Benutzer {Email} wurde erfolgreich erstellt.", user.Email);
                    return true;
                }
                _logger.LogWarning("Erstellung des Benutzers {Email} schlug fehl.", user.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen des Benutzers {Email}.", user.Email);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                int result = await _dbHelper.ExecuteNonQueryAsync(SqlCommands.UpdateUserSql, cmd =>
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    UserMapper.AddUserParameters(cmd, user);
                });
                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) wurde erfolgreich aktualisiert.", user.Id);
                    return true;
                }
                _logger.LogWarning("Aktualisierung des Benutzers (ID: {UserId}) schlug fehl.", user.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Benutzers (ID: {UserId}).", user.Id);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                int result = await _dbHelper.ExecuteNonQueryAsync(SqlCommands.DeleteUserSql, cmd =>
                {
                    cmd.Parameters.AddWithValue("@id", id);
                });
                if (result > 0)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) wurde erfolgreich gelöscht.", id);
                    return true;
                }
                _logger.LogWarning("Kein Benutzer (ID: {UserId}) gefunden, um gelöscht zu werden.", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Benutzers (ID: {UserId}).", id);
                return false;
            }
        }

        public async Task<AuthenticateResult> AuthenticateUserAsync(string emailOrUsername, string password)
        {
            try
            {
                await using var connection = await _dbHelper.GetConnectionAsync();
                await using var command = connection.CreateCommand();
                command.CommandText = SqlCommands.SelectUserByEmailOrUsernameSql;
                command.Parameters.AddWithValue("@value", emailOrUsername);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = UserMapper.CreateUserFromReader(reader, _logger);
                    var inputHash = HashHelper.ComputeHash(password);

                    if (string.Equals(user.PasswordHash, inputHash, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Benutzer {EmailOrUsername} wurde erfolgreich authentifiziert.", emailOrUsername);
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