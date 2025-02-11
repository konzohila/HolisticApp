namespace HolisticApp.Services.Interfaces;

public interface INavigationService
{
    /// <summary>
    /// Navigiert zu einer über den DI‑Container registrierten Seite.
    /// Dabei wird angenommen, dass der Routename der Seite dem Klassennamen entspricht.
    /// </summary>
    /// <typeparam name="TPage">Der Typ der Seite, zu der navigiert werden soll.</typeparam>
    Task NavigateToAsync<TPage>() where TPage : Page;

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