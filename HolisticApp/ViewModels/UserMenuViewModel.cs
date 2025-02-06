using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Storage;
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
            await App.Current.MainPage.DisplayAlert("Einstellungen", "Hier können Sie die Einstellungen öffnen.", "OK");
            await _navigation.PopAsync();
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            Preferences.Remove("LoggedInUserId");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
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

            await App.Current.MainPage.DisplayAlert("Benutzer-Info", infoText, "OK");
        }
    }
}
