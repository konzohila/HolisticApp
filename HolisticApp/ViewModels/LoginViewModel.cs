using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class LoginViewModel(
    IUserRepository userRepository,
    INavigationService navigationService,
    ILogger<LoginViewModel> logger,
    IUserSession userSession)
    : ObservableObject
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly INavigationService _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    private readonly ILogger<LoginViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [ObservableProperty]
    private string _emailOrUsername = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    // Methode, die ausgelöst wird, wenn sich das Email-/Username-Feld ändert
    partial void OnEmailOrUsernameChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Password = string.Empty;
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (currentPage == null)
        {
            _logger.LogError("Kein gültiges Fenster gefunden. Login wird abgebrochen.");
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            await currentPage.DisplayAlert("Fehler", "Bitte Benutzername/Email und Passwort eingeben.", "OK");
            _logger.LogWarning("Login-Versuch fehlgeschlagen: Username/Email oder Passwort wurde nicht ausgefüllt.");
            return;
        }

        _logger.LogInformation("Login-Versuch für Benutzername oder Email: {Input} gestartet.", EmailOrUsername);
        try
        {
            var users = await _userRepository.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                (u.Email.Equals(EmailOrUsername, StringComparison.OrdinalIgnoreCase) || 
                 u.Username?.Equals(EmailOrUsername, StringComparison.OrdinalIgnoreCase) == true) &&
                u.PasswordHash == Password);

            if (user != null)
            {
                Preferences.Set("LoggedInUserId", user.Id);
                userSession.SetUser(user);
                _logger.LogInformation("User (ID: {UserId}) hat sich erfolgreich angemeldet.", user.Id);

                switch (user.Role)
                {
                    case UserRole.Admin:
                        await _navigationService.NavigateToAsync(Routes.AdminDashboardPage);
                        _logger.LogInformation("Navigiere zu AdminDashboardPage.");
                        break;
                    case UserRole.Doctor:
                        await _navigationService.NavigateToAsync(Routes.DoctorDashboardPage);
                        _logger.LogInformation("Navigiere zu DoctorDashboardPage.");
                        break;
                    default:
                        var anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                        if (!anamnesisCompleted)
                        {
                            await _navigationService.NavigateToAsync(Routes.AnamnesisPage);
                            _logger.LogInformation("Navigiere zu AnamnesisPage (Anamnese nicht abgeschlossen).");
                        }
                        else
                        {
                            await _navigationService.NavigateToAsync(Routes.HomePage);
                            _logger.LogInformation("Navigiere zu HomePage (Anamnese abgeschlossen).");
                        }
                        break;
                }
            }
            else
            {
                await currentPage.DisplayAlert("Fehler", "Ungültige Anmeldedaten.", "OK");
                _logger.LogWarning("Anmeldeversuch fehlgeschlagen: Keine Übereinstimmung für Eingabe {Input} gefunden.", EmailOrUsername);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Anmeldeprozess für Eingabe: {Input}", EmailOrUsername);
            await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        try
        {
            _logger.LogInformation("Navigiere zur Registrierungsseite.");
            await _navigationService.NavigateToAsync(Routes.RegistrationPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Wechsel zur Registrierungsseite.");
            var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (currentPage != null)
            {
                await currentPage.DisplayAlert("Fehler", "Ein Fehler beim Navigieren ist aufgetreten.", "OK");
            }
        }
    }
}