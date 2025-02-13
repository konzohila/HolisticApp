using HolisticApp.Constants;
using HolisticApp.Data.Interfaces;
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

        public async Task<bool> SaveUserAsync(string username, string email, string password)
        {
            try
            {
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashHelper.ComputeHash(password),
                    CurrentComplaint = "Keine Beschwerden",
                    Age = null,
                    Gender = "Nicht angegeben",
                    Height = null,
                    Weight = null,
                    Role = UserRole.Patient
                };

                int result = await _dbHelper.ExecuteNonQueryAsync(SqlCommands.InsertUserSql, cmd =>
                {
                    UserMapper.AddUserParameters(cmd, newUser);
                });

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

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("UpdateUserAsync wurde mit null übergeben.");
                return false;
            }

            try
            {
                int result = await _dbHelper.ExecuteNonQueryAsync(SqlCommands.UpdateUserSql, cmd =>
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    UserMapper.AddUserParameters(cmd, user);
                });

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

        public async Task<bool> IsUserInDatabaseAsync(string emailOrUsername)
        {
            try
            {
                var resultObj = await _dbHelper.ExecuteScalarAsync(SqlCommands.CountUserByEmailOrUsernameSql, cmd =>
                {
                    cmd.Parameters.AddWithValue("@value", emailOrUsername);
                });

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

        public async Task<List<User>> FindUsersByRole(UserRole role)
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

        public async Task<AuthenticateResult> AuthenticateUser(string emailOrUsername, string password)
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