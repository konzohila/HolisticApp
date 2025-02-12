using HolisticApp.Data.Interfaces;
using HolisticApp.Enums;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private User? _loggedInUser;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> GetLoggedInUserAsync()
    {
        return _loggedInUser;
    }

    public async Task<List<User>> FindUsersByRole(UserRole role)
    {
        return await _userRepository.FindUsersByRole(role);
    }

    public void LogoutUser()
    {
        _loggedInUser = null;
        Preferences.Remove("LoggedInUserId");
        _logger.LogInformation("Benutzer wurde ausgeloggt.");
    }

    public async Task<bool> IsAnamnesisCompletedAsync()
    {
        return Preferences.Get($"AnamnesisCompleted_{_loggedInUser.Id}", false);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        if (!await _userRepository.UpdateUserAsync(user)) return false;
        _loggedInUser = user;
        return true;
    }

    public async Task<bool> DeleteCurrentUserAsync()
    {
        if (_loggedInUser == null || !await _userRepository.DeleteUserAsync(_loggedInUser.Id)) return false;
        _loggedInUser = null;
        return true;
    }

    public async Task<LoginResult> LoginAsync(string emailOrUsername, string password)
    {
        try
        {
            var exists = await _userRepository.IsUserInDatabaseAsync(emailOrUsername);
            if (exists)
            {
                var result = await _userRepository.AuthenticateUser(emailOrUsername, password);
                if (!result.IsAuthenticated)
                {
                    _logger.LogInformation($"Benutzer {emailOrUsername} wurde erfolgreich eingeloggt");
                    return (new LoginResult(result.User, LoginStatus.Success));
                }
                {
                    _logger.LogInformation($"Für Benutzer {emailOrUsername} wurde beim Loginversuch das falsche Passwort angegeben");
                    return (new LoginResult(result.User, LoginStatus.InvalidPassword));
                }
            }
            _logger.LogInformation($"Es wurde versucht einen unbekannten User einzuloggen: {emailOrUsername}");
            return (new LoginResult(null, LoginStatus.UserNotFound));
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fehler beim Login des Benutzers {emailOrUsername}");
            return (new LoginResult(null, LoginStatus.Error));
        }
    }
    
    public Task<bool> LogoutUserAsync()
    {
        var user = _loggedInUser;
        _loggedInUser = null;
        Preferences.Remove("LoggedInUserId");
        _logger.LogInformation($"Benutzer {user?.Username} wurde ausgeloggt");
        return Task.FromResult(true);
    }

    public async Task<bool> RegisterUserAsync(string username, string email, string password)
    {
        return await _userRepository.SaveUserAsync(username, email, password);
    }
}