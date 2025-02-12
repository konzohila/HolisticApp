using HolisticApp.Models;

namespace HolisticApp.Data.Interfaces;

public interface IUserRepository
{
    Task<bool> SaveUserAsync(string username, string email, string password);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> IsUserInDatabaseAsync(string emailOrUsername);
    Task<List<User>> FindUsersByRole(UserRole role);
    Task<AuthenticateResult> AuthenticateUser(string emailOrUsername, string password);
}