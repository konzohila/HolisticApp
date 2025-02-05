using System;
using System.Linq;
using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        async void OnLoginClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Fehler", "Bitte Email und Passwort eingeben.", "OK");
                return;
            }

            // Benutzer anhand der Eingaben aus der Datenbank suchen
            var users = await App.UserDatabase.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == password);

            if (user != null)
            {
                await DisplayAlert("Erfolg", "Login erfolgreich.", "OK");
                // Navigation zur neuen Anamnese-View mit Übergabe des eingeloggten Benutzers
                await Navigation.PushAsync(new AnamnesisPage(user));
            }
            else
            {
                await DisplayAlert("Fehler", "Ungültige Anmeldedaten.", "OK");
            }
        }
        
        async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Wechsle zur Registrierungsseite
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}