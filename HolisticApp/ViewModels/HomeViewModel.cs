using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _userInitials = string.Empty;

    public HomeViewModel(INavigationService navigationService, IUserService userService, ILogger<HomeViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    public async Task InitializeAsync()
    {
        var user = await UserService.GetLoggedInUserAsync();
        UserInitials = user?.Username?.Substring(0, 1).ToUpper() ?? string.Empty;
    }

    [RelayCommand]
    private async Task OpenAnamnesisAsync() => await NavigationService.NavigateToAsync(Routes.AnamnesisPage);

    [RelayCommand]
    private async Task OpenUserMenuAsync() => await NavigationService.NavigateToAsync(Routes.UserMenuPage);
}