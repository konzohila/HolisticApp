using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public partial class LoginViewModel(
        IUserRepository userRepository,
        INavigation navigation,
        ILogger<LoginViewModel> logger,
        AdminDashboardPage adminDashboardPage,
        DoctorDashboardPage doctorDashboardPage,
        AnamnesisPage anamnesisPage,
        HomePage homePage,
        RegistrationPage registrationPage)
        : ObservableObject
    {
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly INavigation _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        private readonly ILogger<LoginViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (currentPage == null)
            {
                _logger.LogError("Kein gültiges Fenster gefunden. Login wird abgebrochen.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await currentPage.DisplayAlert("Fehler", "Bitte Email und Passwort eingeben.", "OK");
                _logger.LogWarning("Login-Versuch fehlgeschlagen: Email oder Passwort wurden nicht ausgefüllt.");
                return;
            }

            _logger.LogInformation("Login-Versuch für Email: {Email} gestartet.", Email);
            try
            {
                var users = await _userRepository.GetUsersAsync();
                var user = users.FirstOrDefault(u =>
                    u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase) &&
                    u.PasswordHash == Password);

                if (user != null)
                {
                    Preferences.Set("LoggedInUserId", user.Id);
                    _logger.LogInformation("User (ID: {UserId}) hat sich erfolgreich angemeldet.", user.Id);

                    // Navigation basierend auf der Benutzerrolle
                    if (user.Role == UserRole.Admin)
                    {
                        await _navigation.PushAsync(adminDashboardPage);
                        _logger.LogInformation("Navigiere zu AdminDashboardPage.");
                    }
                    else if (user.Role == UserRole.Doctor)
                    {
                        await _navigation.PushAsync(doctorDashboardPage);
                        _logger.LogInformation("Navigiere zu DoctorDashboardPage.");
                    }
                    else
                    {
                        bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                        if (!anamnesisCompleted)
                        {
                            await _navigation.PushAsync(anamnesisPage);
                            _logger.LogInformation("Navigiere zu AnamnesisPage (Anamnese nicht abgeschlossen).");
                        }
                        else
                        {
                            await _navigation.PushAsync(homePage);
                            _logger.LogInformation("Navigiere zu HomePage (Anamnese abgeschlossen).");
                        }
                    }
                }
                else
                {
                    await currentPage.DisplayAlert("Fehler", "Ungültige Anmeldedaten.", "OK");
                    _logger.LogWarning("Anmeldeversuch fehlgeschlagen: Keine Übereinstimmung für Email {Email} gefunden.", Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Anmeldeprozess für Email: {Email}", Email);
                await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            try
            {
                _logger.LogInformation("Navigiere zur Registrierungsseite.");
                await _navigation.PushAsync(registrationPage);
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
}