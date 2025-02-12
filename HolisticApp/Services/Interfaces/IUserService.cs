using HolisticApp.Models;

namespace HolisticApp.Services.Interfaces;

public interface IUserService
{
    Task<LoginResult> LoginAsync (string emailOrUsername, string password);
    Task<bool> LogoutUserAsync();
    Task<bool> RegisterUserAsync(string username, string email, string password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteCurrentUserAsync();
    Task<User?> GetLoggedInUserAsync();
    Task<List<User>> FindUsersByRole(UserRole role);
    Task<bool> IsAnamnesisCompletedAsync();
}