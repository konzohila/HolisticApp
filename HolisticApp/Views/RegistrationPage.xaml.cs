using System;
using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Lese die Eingaben aus
            var username = UsernameEntry.Text;
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            // Erstelle einen neuen User
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = password // In Produktion: Passwort vorher hashen!
            };

            // Speichere den User in der Datenbank
            await App.UserDatabase.SaveUserAsync(user);

            await DisplayAlert("Erfolg", "Benutzer erfolgreich registriert.", "OK");

            // Zurück zur LoginPage navigieren
            await Navigation.PopAsync();
        }
    }
}