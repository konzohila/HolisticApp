using System;
using System.Linq;
using HolisticApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;


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
                // Benutzer persistieren: Speichere z. B. die UserId
                Preferences.Set("LoggedInUserId", user.Id);
                // Optional: ein Flag setzen, das angibt, ob die Anamnese bereits durchgeführt wurde
                // Beispiel: "AnamnesisCompleted" auf false, falls dies noch nicht erfolgt ist
                //Preferences.Set("AnamnesisCompleted", false);

                // Navigation: Falls die Anamnese noch nicht abgeschlossen wurde, zur AnamnesePage, sonst zur HomePage
                bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                if (!anamnesisCompleted)
                {
                    await Navigation.PushAsync(new AnamnesisPage(user));
                }
                else
                {
                    await Navigation.PushAsync(new HomePage(user));
                }
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