using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public RegistrationViewModel(IUserRepository userRepository, INavigation navigation)
        {
            _userRepository = userRepository;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password // In Produktion bitte hashen!
            };

            await _userRepository.SaveUserAsync(user);
            await App.Current.MainPage.DisplayAlert("Erfolg", "Benutzer erfolgreich registriert.", "OK");
            await _navigation.PopAsync();
        }
    }
}