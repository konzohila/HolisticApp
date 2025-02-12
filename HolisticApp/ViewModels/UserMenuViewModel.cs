using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class UserMenuViewModel : BaseViewModel
{
    public UserMenuViewModel(INavigationService navigationService, IUserService userService, ILogger<UserMenuViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    [RelayCommand]
    private async Task ShowInfo() => await NavigationService.NavigateToAsync(Routes.UserInfoPage);

    [RelayCommand]
    private async Task ShowSettings() => await NavigationService.NavigateToAsync(Routes.SettingsPage);

    [RelayCommand]
    private async Task Logout()
    {
        await UserService.LogoutUserAsync();
        await NavigationService.NavigateToAsync(Routes.LoginPage);
    }
}