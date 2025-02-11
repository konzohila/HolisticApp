using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Services;

public class NavigationService(ILogger<NavigationService> logger) : INavigationService
{
    private readonly ILogger<NavigationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task NavigateToAsync<TPage>() where TPage : Page
    {
        try
        {
            // Annahme: Der Routename entspricht dem Namen der Seite.
            string route = $"//{typeof(TPage).Name}";
            _logger.LogInformation("Navigiere zu Seite {Route}", route);
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Navigieren zur Seite vom Typ {PageType}", typeof(TPage).Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(string route)
    {
        try
        {
            _logger.LogInformation("Navigiere zu Route {Route}", route);
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Navigieren zu Route {Route}", route);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task GoBackAsync()
    {
        try
        {
            _logger.LogInformation("Navigiere zurück");
            // ".." navigiert eine Ebene zurück
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Navigieren zurück");
            throw;
        }
    }
}