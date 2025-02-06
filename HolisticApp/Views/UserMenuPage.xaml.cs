using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace HolisticApp.Views
{
    public partial class UserMenuPage : ContentPage
    {
        private string _fullName;

        public UserMenuPage(string fullName)
        {
            InitializeComponent();
            _fullName = fullName;
            FullNameLabel.Text = fullName; // Benutzername anzeigen
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
    }
}