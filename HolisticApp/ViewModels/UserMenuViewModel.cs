using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public partial class UserMenuViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly ILogger<UserMenuViewModel> _logger;

        public User CurrentUser { get; }

        public UserMenuViewModel(User user,
                                 INavigation navigation,
                                 ILogger<UserMenuViewModel> logger)
        {
            CurrentUser = user;
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                _logger.LogInformation("User {UserId} öffnet die Einstellungen.", CurrentUser.Id);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen der Einstellungen für User {UserId}", CurrentUser.Id);
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
                _logger.LogInformation("User {UserId} hat sich ausgeloggt.", CurrentUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Logout für User {UserId}", CurrentUser.Id);
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task ShowInfoAsync()
        {
            try
            {
                string complaintInfo = string.IsNullOrEmpty(CurrentUser.CurrentComplaint) || CurrentUser.CurrentComplaint == "Keine Beschwerden"
                    ? "Aktuell keine Beschwerden gespeichert."
                    : $"Aktuelle Beschwerde: {CurrentUser.CurrentComplaint}";

                string ageInfo = CurrentUser.Age.HasValue ? $"Alter: {CurrentUser.Age} Jahre" : "Alter: Nicht angegeben";
                string genderInfo = !string.IsNullOrEmpty(CurrentUser.Gender) ? $"Geschlecht: {CurrentUser.Gender}" : "Geschlecht: Nicht angegeben";
                string heightInfo = CurrentUser.Height.HasValue ? $"Größe: {CurrentUser.Height} cm" : "Größe: Nicht angegeben";
                string weightInfo = CurrentUser.Weight.HasValue ? $"Gewicht: {CurrentUser.Weight} kg" : "Gewicht: Nicht angegeben";

                string infoText = $"{ageInfo}\n{genderInfo}\n{heightInfo}\n{weightInfo}\n\n{complaintInfo}";

                var window = Application.Current?.Windows?.FirstOrDefault();
                if (window?.Page != null)
                {
                    await window.Page.DisplayAlert("Benutzer-Info", infoText, "OK");
                }
                _logger.LogInformation("User {UserId} ruft sein Info-Fenster auf.", CurrentUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Anzeigen der Benutzer-Info für User {UserId}", CurrentUser.Id);
            }
        }
    }
}