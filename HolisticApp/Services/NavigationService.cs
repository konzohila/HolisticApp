using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
    }

    public async Task NavigateToAsync(string route)
    {
        try
        {
            if (Shell.Current != null)
            {
                _logger.LogInformation("Navigiere zu: {Route}", route);
                await Shell.Current.GoToAsync(route);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Navigation zu: {Route}", route);
        }
    }

    public async Task GoBackAsync()
    {
        try
        {
            if (Shell.Current?.Navigation?.NavigationStack.Count > 1)
            {
                _logger.LogInformation("Navigiere zurück.");
                await Shell.Current.Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Navigation zurück.");
        }
    }
}