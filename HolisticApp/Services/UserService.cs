using HolisticApp.Models;
using HolisticApp.Helpers;
using HolisticApp.Enums;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private User _loggedInUser;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            _logger.LogInformation("Lade Benutzer mit ID {UserId}...", id);
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Kein Benutzer mit ID {UserId} gefunden.", id);
            }
            return user;
        }

        public async Task<List<User>> GetUsersAsync(UserRole role)
        {
            _logger.LogInformation("Lade alle Benutzer mit der Rolle {Role}...", role);
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.ToList();
        }

        public async Task<List<User>> GetUsersByMasterIdAsync(int masterId)
        {
            _logger.LogInformation("Lade alle Benutzer, die dem MasterAccount mit ID {MasterId} zugeordnet sind...", masterId);
            var allPatients = await _userRepository.GetUsersByRoleAsync(UserRole.Patient);
            var filtered = allPatients.Where(u => u.MasterAccountId == masterId).ToList();
            if (!filtered.Any())
            {
                _logger.LogInformation("Keine Benutzer für MasterAccount ID {MasterId} gefunden.", masterId);
            }
            return filtered;
        }

        public async Task<LoginResult> LoginAsync(string emailOrUsername, string password)
        {
            _logger.LogInformation("Versuche Login für {Identifier}...", emailOrUsername);
            var authResult = await _userRepository.AuthenticateUserAsync(emailOrUsername, password);
            if (authResult.IsAuthenticated)
            {
                _loggedInUser = authResult.User;
                _logger.LogInformation("Login für {Identifier} erfolgreich.", emailOrUsername);
                return new LoginResult(authResult.User, LoginStatus.Success);
            }
            else
            {
                _logger.LogWarning("Login fehlgeschlagen für {Identifier}.", emailOrUsername);
                return new LoginResult(null, LoginStatus.InvalidPassword);
            }
        }

        public async Task<LoginResult> LoginAsync(int id)
        {
            _logger.LogInformation("Versuche Login für Benutzer mit ID {UserId}...", id);
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user != null)
            {
                _logger.LogInformation("Benutzer mit ID {UserId} gefunden. Login erfolgreich.", id);
                _loggedInUser = user;
                return new LoginResult(user, LoginStatus.Success);
            }
            else
            {
                _logger.LogWarning("Kein Benutzer mit ID {UserId} gefunden. Login fehlgeschlagen.", id);
                return new LoginResult(null, LoginStatus.UserNotFound);
            }
        }

        public Task<bool> LogoutCurrentUserAsync()
        {
            _logger.LogInformation("Lösche den aktuell eingeloggten Benutzer aus den Preferences.");
            try
            {
                Preferences.Remove("LoggedInUserId");
                _loggedInUser = null;
                _logger.LogInformation("Logout erfolgreich durchgeführt.");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Logout des aktuellen Benutzers.");
                return Task.FromResult(false);
            }
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password, UserRole role)
        {
            _logger.LogInformation("Registriere neuen Benutzer: {Email}", email);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashHelper.ComputeHash(password),
                CurrentComplaint = "Keine Beschwerden",
                Age = null,
                Gender = "Nicht angegeben",
                Height = null,
                Weight = null,
                Role = role
            };

            bool result = await _userRepository.CreateUserAsync(user);
            if (result)
            {
                _logger.LogInformation("Registrierung von {Email} erfolgreich.", email);
            }
            else
            {
                _logger.LogError("Registrierung von {Email} schlug fehl.", email);
            }
            return result;
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            _logger.LogInformation("Aktualisiere Benutzer (ID: {UserId})...", user.Id);
            return _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteCurrentUserAsync()
        {
            _logger.LogInformation("Versuche den aktuell eingeloggten Benutzer zu löschen...");
            if (_loggedInUser == null)
            {
                _logger.LogWarning("Kein eingeloggter Benutzer gefunden, um gelöscht zu werden.");
                return false;
            }
            bool result = await _userRepository.DeleteUserAsync(_loggedInUser.Id);
            if (result)
            {
                _loggedInUser = null;
                Preferences.Remove("LoggedInUserId");
                _logger.LogInformation("Benutzer (ID: {UserId}) wurde erfolgreich gelöscht.", _loggedInUser.Id);
            }
            else
            {
                _logger.LogError("Löschen des Benutzers (ID: {UserId}) schlug fehl.", _loggedInUser.Id);
            }
            return result;
        }

        public async Task<User?> GetLoggedInUserAsync()
        {
            if (_loggedInUser != null)
            {
                return _loggedInUser;
            }
            
            _logger.LogInformation("Lese aktuell eingeloggten Benutzer aus den Preferences...");
            if (Preferences.ContainsKey("LoggedInUserId"))
            {
                int userId = Preferences.Get("LoggedInUserId", 0);
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user != null)
                {
                    _logger.LogInformation("Benutzer (ID: {UserId}) wurde gefunden.", userId);
                }
                else
                {
                    _logger.LogWarning("Kein Benutzer mit ID {UserId} in der Datenbank gefunden.", userId);
                }
                _loggedInUser = user;
                return user;
            }
            _logger.LogWarning("Kein Login-Eintrag in den Preferences gefunden.");
            return null;
        }

        public async Task<List<User>> FindUsersByRoleAsync(UserRole role)
        {
            _logger.LogInformation("Finde alle Benutzer mit der Rolle {Role}...", role);
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.ToList();
        }

        public Task<bool> IsAnamnesisCompletedAsync()
        {
            bool completed = Preferences.Get($"AnamnesisCompleted_{_loggedInUser.Id}", false);
            _logger.LogInformation($"AnamnesisCompleted_{_loggedInUser.Id}: {{Status}}", completed);
            return Task.FromResult(completed);
        }

        public void SetAnamnesisCompleted(bool completed)
        {
            Preferences.Set($"AnamnesisCompleted_{_loggedInUser.Id}", completed);
            _logger.LogInformation($"AnamnesisCompleted für User mit ID{_loggedInUser.Id} in den Preferences auf {{Status}} gesetzt.", completed);
        }
    }
}