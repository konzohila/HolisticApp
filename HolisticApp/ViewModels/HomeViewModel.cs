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
        private string userInitials;

        public HomeViewModel(User user, INavigation navigation)
        {
            CurrentUser = user;
            _navigation = navigation;
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