using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using HolisticApp.Models;
using Microsoft.Extensions.Logging;

namespace HolisticApp;

public partial class App
{
    private readonly IUserService _userService;
    private readonly ILogger<App> _logger;
    private readonly INavigationService _navigationService;

    [Obsolete("Obsolete")]
    public App(IUserService userService, ILogger<App> logger, INavigationService navigationService)
    {
        InitializeComponent();
        _userService = userService;
        _logger = logger;
        _navigationService = navigationService;
        _logger.LogInformation("Die App wurde gestartet.");

        MainPage = new AppShell();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            await Task.Delay(500);
            var user = await _userService.GetLoggedInUserAsync();
            if (user == null)
            {
                _logger.LogInformation("[App] Kein Benutzer angemeldet. Navigiere zur Login-Seite.");
                await _navigationService.NavigateToAsync(Routes.LoginPage);
                return;
            }

            _logger.LogInformation("[App] User {UserId} ({Role}) gefunden.", user.Id, user.Role);
            switch (user.Role)
            {
                case UserRole.Doctor:
                    await _navigationService.NavigateToAsync(Routes.DoctorDashboardPage);
                    break;
                case UserRole.Admin:
                    await _navigationService.NavigateToAsync(Routes.AdminDashboardPage);
                    break;
                default:
                    var anamnesisCompleted = await _userService.IsAnamnesisCompletedAsync();
                    await _navigationService.NavigateToAsync(anamnesisCompleted ? Routes.HomePage : Routes.AnamnesisPage);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[App] Fehler während der Initialisierung.");
            await _navigationService.NavigateToAsync(Routes.LoginPage);
        }
    }
}