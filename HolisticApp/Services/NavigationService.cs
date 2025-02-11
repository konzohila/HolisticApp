using Microsoft.Extensions.Logging;
using HolisticApp.Services.Interfaces;

namespace HolisticApp.Services
{
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
                _logger.LogInformation("Navigiere zu: {Route}", route);
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der Navigation zu {Route}", route);
            }
        }

        public async Task GoBackAsync()
        {
            try
            {
                if (Shell.Current.CurrentPage is Views.RegistrationPage)
                {
                    _logger.LogInformation("Navigiere explizit zur LoginPage zurück.");
                    await Shell.Current.GoToAsync("//LoginPage"); // Feste Navigation zur LoginPage
                    return;
                }
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                {
                    _logger.LogInformation("Navigiere zurück zur vorherigen Seite.");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    _logger.LogWarning("Kein vorheriger Navigationseintrag vorhanden, zurück zur Startseite.");
                    await Shell.Current.GoToAsync("//HomePage"); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der Zurück-Navigation.");
            }
        }
    }
}