using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Services.Interfaces;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class UserMenuViewModel(
    INavigationService navigationService,
    ILogger<UserMenuViewModel> logger,
    IUserSession userSession)
    : ObservableObject
{
    private readonly INavigationService _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    private readonly ILogger<UserMenuViewModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [RelayCommand]
    public async Task OpenSettingsAsync()
    {
        try
        {
            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlert("Einstellungen", "Hier können Sie die Einstellungen öffnen.", "OK");
            }
            _logger.LogInformation("User {UserId} öffnet die Einstellungen.", userSession.CurrentUser.Id);
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Öffnen der Einstellungen für User {UserId}", userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    public Task LogoutAsync()
    {
        try
        {
            Preferences.Remove("LoggedInUserId");
            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window != null)
            {
                if (Application.Current?.Handler != null)
                {
                    var services = Application.Current.Handler.MauiContext?.Services;
                    if (services != null)
                    {
                        var loginPage = services.GetRequiredService<LoginPage>();
                        window.Page = loginPage;
                    }
                }
            }
            _logger.LogInformation("User {UserId} hat sich ausgeloggt.", userSession.CurrentUser.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Logout für User {UserId}", userSession.CurrentUser.Id);
        }
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ShowInfoAsync()
    {
        try
        {
            var complaintInfo = string.IsNullOrEmpty(userSession.CurrentUser.CurrentComplaint) || userSession.CurrentUser.CurrentComplaint == "Keine Beschwerden"
                ? "Aktuell keine Beschwerden gespeichert."
                : $"Aktuelle Beschwerde: {userSession.CurrentUser.CurrentComplaint}";

            var ageInfo = userSession.CurrentUser.Age.HasValue ? $"Alter: {userSession.CurrentUser.Age} Jahre" : "Alter: Nicht angegeben";
            var genderInfo = !string.IsNullOrEmpty(userSession.CurrentUser.Gender) ? $"Geschlecht: {userSession.CurrentUser.Gender}" : "Geschlecht: Nicht angegeben";
            var heightInfo = userSession.CurrentUser.Height.HasValue ? $"Größe: {userSession.CurrentUser.Height} cm" : "Größe: Nicht angegeben";
            var weightInfo = userSession.CurrentUser.Weight.HasValue ? $"Gewicht: {userSession.CurrentUser.Weight} kg" : "Gewicht: Nicht angegeben";

            var infoText = $"{ageInfo}\n{genderInfo}\n{heightInfo}\n{weightInfo}\n\n{complaintInfo}";

            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlert("Benutzer-Info", infoText, "OK");
            }
            _logger.LogInformation("User {UserId} ruft sein Info-Fenster auf.", userSession.CurrentUser.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Anzeigen der Benutzer-Info für User {UserId}", userSession.CurrentUser.Id);
        }
    }
}