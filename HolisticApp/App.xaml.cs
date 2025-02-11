using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using Microsoft.Extensions.Logging;

namespace HolisticApp;

public partial class App
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<App> _logger;

    [Obsolete("Obsolete")]
    public App(IUserRepository userRepository, ILogger<App> logger)
    {
        InitializeComponent();
        _userRepository = userRepository;
        _logger = logger;
        _logger.LogInformation("Die App wurde gestartet.");

        // Setze die Shell als MainPage
        MainPage = new AppShell();

        // Starte die initiale Navigation
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            var userId = Preferences.Get("LoggedInUserId", 0);
            if (userId <= 0)
            {
                _logger.LogInformation("[App] Kein Benutzer angemeldet. Navigiere zur Login-Seite.");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            var user = await _userRepository.GetUserAsync(userId);
            if (user == null)
            {
                _logger.LogError("[App] Kein User für die gespeicherte ID gefunden. Navigiere zur Login-Seite.");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Navigiere basierend auf der Benutzerrolle
            switch (user.Role)
            {
                case UserRole.Doctor:
                    _logger.LogInformation("[App] User {UserId} (Doctor) gefunden. Navigiere zur DoctorDashboardPage.", user.Id);
                    await Shell.Current.GoToAsync("//DoctorDashboardPage");
                    break;
                case UserRole.Admin:
                    _logger.LogInformation("[App] User {UserId} (Admin) gefunden. Navigiere zur AdminDashboardPage.", user.Id);
                    await Shell.Current.GoToAsync("//AdminDashboardPage");
                    break;
                default:
                    var anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                    _logger.LogInformation("[App] User {UserId} (Patient) gefunden. Anamnese abgeschlossen: {AnamnesisCompleted}", user.Id, anamnesisCompleted);
                    if (anamnesisCompleted)
                        await Shell.Current.GoToAsync("//HomePage");
                    else
                        await Shell.Current.GoToAsync("//AnamnesisPage");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[App] Fehler während der Initialisierung");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}