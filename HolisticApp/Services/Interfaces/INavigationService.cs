namespace HolisticApp.Services.Interfaces;

public interface INavigationService
{
    /// <summary>
    /// Navigiert zu einer Seite über eine explizite Route.
    /// </summary>
    /// <param name="route">Die Shell‑Route (z. B. "//LoginPage").</param>
    Task NavigateToAsync(string route);

    /// <summary>
    /// Geht im Navigationsstack eine Seite zurück.
    /// </summary>
    Task GoBackAsync();
}