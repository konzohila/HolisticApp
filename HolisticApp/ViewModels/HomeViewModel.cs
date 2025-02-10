using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly ILogger<HomeViewModel> _logger;
        [ObservableProperty]
        private string _userInitials = string.Empty;
        private User CurrentUser { get; }
        private AnamnesisPage _anamnesisPage;
        private UserMenuPage _userMenuPage;

        public HomeViewModel(User user,
                             INavigation navigation,
                             ILogger<HomeViewModel> logger,
                             AnamnesisPage anamnesisPage,
                             UserMenuPage userMenuPage)
        {
            _anamnesisPage = anamnesisPage;
            _userMenuPage = userMenuPage;
            CurrentUser = user;
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Initialisierung
            UserInitials = !string.IsNullOrWhiteSpace(user.Username)
                ? user.Username[..1].ToUpper()
                : string.Empty;
        }

        [RelayCommand]
        public async Task OpenAnamnesisAsync()
        {
            try
            {
                _logger.LogInformation("Öffne AnamnesisPage für User {UserId}", CurrentUser.Id);
                await _navigation.PushAsync(_anamnesisPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen der AnamnesisPage für User {UserId}", CurrentUser.Id);
            }
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            try
            {
                _logger.LogInformation("Öffne UserMenuPage für User {UserId}", CurrentUser.Id);
                await _navigation.PushAsync(_userMenuPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für User {UserId}", CurrentUser.Id);
            }
        }
    }
}