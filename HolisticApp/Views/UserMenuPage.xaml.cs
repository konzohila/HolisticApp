using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using HolisticApp.Models;

namespace HolisticApp.Views
{
    public partial class UserMenuPage : ContentPage
    {
        private User _currentUser;

        public UserMenuPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            FullNameLabel.Text = _currentUser.Username; // Setzt den Benutzernamen in der UI
        }
        
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // Beispiel: Navigation zur Einstellungen-Seite (kann später angepasst werden)
            await DisplayAlert("Einstellungen", "Hier können Sie die Einstellungen öffnen.", "OK");
            await Navigation.PopAsync(); // Menü schließen
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Logout-Logik: Benutzer-Daten aus Preferences löschen
            Preferences.Remove("LoggedInUserId");

            // Navigiere zurück zur Login-Seite
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        
        private async void OnInfoClicked(object sender, EventArgs e)
        {
            string complaintInfo = string.IsNullOrEmpty(_currentUser.CurrentComplaint) || _currentUser.CurrentComplaint == "Keine Beschwerden"
                ? "Aktuell keine Beschwerden gespeichert."
                : $"Aktuelle Beschwerde: {_currentUser.CurrentComplaint}";

            string ageInfo = _currentUser.Age.HasValue ? $"Alter: {_currentUser.Age} Jahre" : "Alter: Nicht angegeben";
            string genderInfo = !string.IsNullOrEmpty(_currentUser.Gender) ? $"Geschlecht: {_currentUser.Gender}" : "Geschlecht: Nicht angegeben";
            string heightInfo = _currentUser.Height.HasValue ? $"Größe: {_currentUser.Height} cm" : "Größe: Nicht angegeben";
            string weightInfo = _currentUser.Weight.HasValue ? $"Gewicht: {_currentUser.Weight} kg" : "Gewicht: Nicht angegeben";

            string infoText = $"{ageInfo}\n{genderInfo}\n{heightInfo}\n{weightInfo}\n\n{complaintInfo}";

            await DisplayAlert("Benutzer-Info", infoText, "OK");
        }
    }
}