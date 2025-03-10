using HolisticApp.Models;

public interface IUserService
{
    Task<User?> GetUserAsync(int id);
    Task<List<User>> GetUsersAsync(UserRole role);
    Task<List<User>> GetUsersByMasterIdAsync(int masterId);
    Task<LoginResult> LoginAsync(string emailOrUsername, string password);
    Task<LoginResult> LoginAsync(int id);
    Task<bool> LogoutCurrentUserAsync();
    Task<bool> RegisterUserAsync(string username, string email, string password, UserRole role);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteCurrentUserAsync();
    Task<User?> GetLoggedInUserAsync();
    Task<List<User>> FindUsersByRoleAsync(UserRole role);
    Task<bool> IsAnamnesisCompletedAsync();
    void SetAnamnesisCompleted(bool completed);
}