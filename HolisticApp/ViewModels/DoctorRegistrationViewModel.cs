using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;

namespace HolisticApp.ViewModels
{
    public partial class DoctorRegistrationViewModel(IUserRepository userRepository, INavigation navigation)
        : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            var currentPage = Application.Current?.Windows[0].Page;
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            var doctor = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password, // Hinweis: In Produktion bitte vorher hashen!
                Role = UserRole.Doctor,
                MasterAccountId = null
            };

            int result = await userRepository.SaveUserAsync(doctor);
            if (result > 0)
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Erfolg", "Doktor erfolgreich registriert.", "OK");
                await navigation.PopAsync();
            }
            else
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Fehler beim Registrieren des Doktors.", "OK");
            }
        }
    }
}