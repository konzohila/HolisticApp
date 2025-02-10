using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class UserMenuViewModel : ObservableObject
    {
        public User CurrentUser { get; }
        private readonly INavigation _navigation;

        public UserMenuViewModel(User user, INavigation navigation)
        {
            CurrentUser = user;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task OpenSettingsAsync()
        {
            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window?.Page != null)
            {
                await window.Page.DisplayAlert("Einstellungen", "Hier können Sie die Einstellungen öffnen.", "OK");
            }
            await _navigation.PopAsync();
        }

        [RelayCommand]
        public Task LogoutAsync()
        {
            Preferences.Remove("LoggedInUserId");
            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window != null)
            {
                window.Page = new NavigationPage(new LoginPage());
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task ShowInfoAsync()
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
        }
    }
}