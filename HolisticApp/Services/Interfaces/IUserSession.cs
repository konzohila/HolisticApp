using HolisticApp.Models;

namespace HolisticApp.Services.Interfaces
{
    public interface IUserSession
    {
        /// <summary>
        /// Enthält den aktuell eingeloggten Benutzer (oder null, wenn niemand angemeldet ist).
        /// </summary>
        User? CurrentUser { get; set; }

        /// <summary>
        /// Gibt an, ob ein Benutzer angemeldet ist.
        /// </summary>
        bool IsUserLoggedIn { get; }

        /// <summary>
        /// Setzt den aktuell eingeloggten Benutzer.
        /// </summary>
        /// <param name="user">Der eingeloggte Benutzer.</param>
        void SetUser(User user);

        /// <summary>
        /// Entfernt den aktuell eingeloggten Benutzer (z. B. beim Logout).
        /// </summary>
        void ClearUser();
    }
}