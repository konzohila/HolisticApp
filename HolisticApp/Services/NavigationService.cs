using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<string> _navigationStack = new();

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
                _navigationStack.Push(Shell.Current.CurrentState.Location.OriginalString); 
                await Shell.Current.GoToAsync(route);
                _logger.LogInformation("Navigiere zu: {Route}", route);
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
            // Falls ein Modal offen ist, schließe es zuerst
            if (Shell.Current.Navigation.ModalStack.Count > 0)
            {
                await Shell.Current.Navigation.PopModalAsync();
                return;
            }

            // Falls eine vorherige Seite existiert, navigiere zurück
            if (_navigationStack.Count > 0)
            {
                string previousPage = _navigationStack.Pop();
                await Shell.Current.GoToAsync(previousPage);
                return;
            }
            _logger.LogInformation("Navigiere zurück.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei Navigation zurück.");
        }
    }
}