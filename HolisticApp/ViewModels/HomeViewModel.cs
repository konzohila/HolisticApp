using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;

namespace HolisticApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private User CurrentUser { get; }

        [ObservableProperty]
        private string _userInitials = string.Empty; // Standardwert

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
        private async Task OpenAnamnesisAsync()
        {
            await _navigation.PushAsync(new AnamnesisPage(CurrentUser));
        }

        [RelayCommand]
        private async Task OpenUserMenuAsync()
        {
            await _navigation.PushAsync(new UserMenuPage(CurrentUser));
        }
    }
}