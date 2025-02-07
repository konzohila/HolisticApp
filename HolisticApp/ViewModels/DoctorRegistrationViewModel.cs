using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class DoctorRegistrationViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;

        public DoctorRegistrationViewModel(IUserRepository userRepository, INavigation navigation)
        {
            _userRepository = userRepository;
            _navigation = navigation;
        }

        // Diese Properties werden per Binding in der XAML genutzt.
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        // Der Command, der beim Klick auf den Registrierungs-Button ausgelöst wird.
        [RelayCommand]
        public async Task RegisterAsync()
        {
            // Überprüfe, ob alle Felder gefüllt sind.
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            // Erstelle einen neuen Doktor-Account.
            var doctor = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password, // In Produktion bitte natürlich das Passwort vorher hashen!
                Role = UserRole.Doctor,  // Setze explizit die Rolle auf Doctor.
                MasterAccountId = null   // Doktoren sind Master-Accounts, daher kein zugeordnetes MasterAccountId.
            };

            // Speichere den neuen Benutzer in der Datenbank.
            int result = await _userRepository.SaveUserAsync(doctor);
            if (result > 0)
            {
                await App.Current.MainPage.DisplayAlert("Erfolg", "Doktor erfolgreich registriert.", "OK");
                await _navigation.PopAsync();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Fehler beim Registrieren des Doktors.", "OK");
            }
        }
    }
}
