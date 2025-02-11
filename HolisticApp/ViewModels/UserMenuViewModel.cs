using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class UserMenuViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<UserMenuViewModel> _logger;
    private readonly IUserSession _userSession;
    
    public UserMenuViewModel(
        INavigationService navigationService,
        ILogger<UserMenuViewModel> logger,
        IUserSession userSession)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        
        if (userSession.CurrentUser == null)
        {
            throw new InvalidOperationException("Kein angemeldeter Benutzer gefunden!");
        } 
        
        // Initialisierung auf dem Hauptthread ausführen
        MainThread.InvokeOnMainThreadAsync(InitializeAsync);
    }

    private async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("UserMenuViewModel wird initialisiert...");
            await Task.Delay(500).ConfigureAwait(false);
            _logger.LogInformation("Initialisierung abgeschlossen.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei der Initialisierung von UserMenuViewModel.");
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        try
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlert("Einstellungen", "Hier können Sie die Einstellungen öffnen.", "OK");
            }

            if (_userSession.CurrentUser != null)
                _logger.LogInformation("User {UserId} öffnet die Einstellungen.", _userSession.CurrentUser.Id);
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler beim Öffnen der Einstellungen für User {UserId}",
                    _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        try
        {
            Preferences.Remove("LoggedInUserId");
            await Shell.Current.GoToAsync("//LoginPage");
            _userSession.ClearUser();
            if (_userSession.CurrentUser != null)
                _logger.LogInformation("User {UserId} hat sich ausgeloggt.", _userSession.CurrentUser.Id);
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler beim Logout für User {UserId}", _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    private async Task ShowInfoAsync()
    {
        try
        {
            var user = _userSession.CurrentUser;
            var complaintInfo = string.IsNullOrEmpty(user?.CurrentComplaint) || user.CurrentComplaint == "Keine Beschwerden"
                ? "Aktuell keine Beschwerden gespeichert."
                : $"Aktuelle Beschwerde: {user.CurrentComplaint}";

            var ageInfo = user is { Age: not null } ? $"Alter: {user.Age} Jahre" : "Alter: Nicht angegeben";
            var genderInfo = !string.IsNullOrEmpty(user?.Gender) ? $"Geschlecht: {user.Gender}" : "Geschlecht: Nicht angegeben";
            var heightInfo = user is { Height: not null } ? $"Größe: {user.Height} cm" : "Größe: Nicht angegeben";
            var weightInfo = user is { Weight: not null } ? $"Gewicht: {user.Weight} kg" : "Gewicht: Nicht angegeben";

            var infoText = $"{ageInfo}\n{genderInfo}\n{heightInfo}\n{weightInfo}\n\n{complaintInfo}";

            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlert("Benutzer-Info", infoText, "OK");
            }

            if (user != null) _logger.LogInformation("User {UserId} ruft sein Info-Fenster auf.", user.Id);
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler beim Anzeigen der Benutzer-Info für User {UserId}",
                    _userSession.CurrentUser.Id);
        }
    }
}