using HolisticApp.Models;

public interface IUserRepository
{
    /// <summary>
    /// Liest einen Benutzer anhand seiner ID.
    /// </summary>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Liest alle Benutzer mit der angegebenen Rolle.
    /// </summary>
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

    /// <summary>
    /// Legt einen neuen Benutzer in der Datenbank an.
    /// </summary>
    Task<bool> CreateUserAsync(User user);

    /// <summary>
    /// Aktualisiert einen bestehenden Benutzer in der Datenbank.
    /// </summary>
    Task<bool> UpdateUserAsync(User user);

    /// <summary>
    /// Löscht einen Benutzer anhand seiner ID.
    /// </summary>
    Task<bool> DeleteUserAsync(int id);

    /// <summary>
    /// Authentifiziert einen Benutzer anhand von Email/Username und Passwort.
    /// </summary>
    Task<AuthenticateResult> AuthenticateUserAsync(string emailOrUsername, string password);
}