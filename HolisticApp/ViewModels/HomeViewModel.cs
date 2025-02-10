using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        public User CurrentUser { get; }

        [ObservableProperty]
        private string userInitials = string.Empty; // Standardwert

        public HomeViewModel(User user, INavigation navigation)
        {
            CurrentUser = user;
            _navigation = navigation;
            // Optional: Du kannst hier auch gleich initialisieren, falls du das Ergebnis berechnen möchtest.
            UserInitials = !string.IsNullOrWhiteSpace(user.Username)
                ? user.Username.Substring(0, 1).ToUpper()
                : string.Empty;
        }

        [RelayCommand]
        public async Task OpenAnamnesisAsync()
        {
            await _navigation.PushAsync(new AnamnesisPage(CurrentUser));
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            await _navigation.PushAsync(new UserMenuPage(CurrentUser));
        }
    }
}