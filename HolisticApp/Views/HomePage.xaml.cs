using Microsoft.Maui.Controls;
using HolisticApp.Models;
using Microsoft.Maui.Storage;

namespace HolisticApp.Views
{
    public partial class HomePage : ContentPage
    {
        private User _currentUser;
        public HomePage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            // Optional: Anzeigen des Benutzernamens oder andere Informationen
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Entferne den gespeicherten Benutzer
            Preferences.Remove("LoggedInUserId");
            Preferences.Remove("AnamnesisCompleted");

            // Navigiere zurück zur LoginPage (evtl. NavigationStack leeren)
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}