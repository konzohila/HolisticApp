using HolisticApp.Models;

namespace HolisticApp.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserAsync(int id); // Nullable Rückgabetyp, da kein User gefunden werden kann.
        Task<int> SaveUserAsync(User user);
        Task<int> DeleteUserAsync(int id);
    }
}