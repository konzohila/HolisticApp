using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(IUserRepository userRepository, INavigation navigation)
        {
            _userRepository = userRepository;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Bitte Email und Passwort eingeben.", "OK");
                return;
            }

            var users = await _userRepository.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(Email, System.StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == Password);

            if (user != null)
            {
                Preferences.Set("LoggedInUserId", user.Id);
                bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                if (!anamnesisCompleted)
                    await _navigation.PushAsync(new AnamnesisPage(user));
                else
                    await _navigation.PushAsync(new HomePage(user));
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Ungültige Anmeldedaten.", "OK");
            }
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            await _navigation.PushAsync(new RegistrationPage());
        }
    }
}
