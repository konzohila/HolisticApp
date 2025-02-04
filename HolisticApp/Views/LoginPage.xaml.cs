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

            // Lese alle Benutzer aus der Datenbank und suche den, der den Eingaben entspricht.
            var users = await App.UserDatabase.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == password);

            if (user != null)
            {
                await DisplayAlert("Erfolg", "Login erfolgreich.", "OK");
                // Hier kannst du zur Hauptseite der App navigieren
                // z.B.: await Navigation.PushAsync(new MainPage());
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