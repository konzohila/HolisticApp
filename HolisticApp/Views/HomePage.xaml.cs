using Microsoft.Maui.Controls;
using HolisticApp.Models;
using HolisticApp.Views;

namespace HolisticApp.Views
{
    public partial class HomePage : ContentPage
    {
        private User _currentUser;

        public HomePage(User currentUser)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            _currentUser = currentUser;

            // Initialen des Benutzers in der Toolbar anzeigen
            if (_currentUser != null && !string.IsNullOrWhiteSpace(_currentUser.Username))
            {
                UserToolbarItem.Text = GetInitials(_currentUser.Username);
            }
        }

        private string GetInitials(string fullName)
        {
            // Initialen des Benutzers berechnen, z. B. "Max Mustermann" -> "MM"
            var parts = fullName.Split(' ');
            if (parts.Length == 0) return "";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();

            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }
        
        private async void OnUserToolbarItemClicked(object sender, EventArgs e)
        {
            string fullName = _currentUser?.Username ?? "Unbekannt";
            await Navigation.PushAsync(new UserMenuPage(fullName)); 
        }
    }
}