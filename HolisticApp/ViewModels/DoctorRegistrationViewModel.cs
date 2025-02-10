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

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            var currentPage = Application.Current?.Windows?[0]?.Page;
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

            int result = await _userRepository.SaveUserAsync(doctor);
            if (result > 0)
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Erfolg", "Doktor erfolgreich registriert.", "OK");
                await _navigation.PopAsync();
            }
            else
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Fehler beim Registrieren des Doktors.", "OK");
            }
        }
    }
}