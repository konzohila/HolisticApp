using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _notificationsEnabled;

    public SettingsViewModel(INavigationService navigationService, IUserService userService, ILogger<SettingsViewModel> logger)
        : base(navigationService, userService, logger)
    {
        NotificationsEnabled = Preferences.Get("NotificationsEnabled", true);
    }

    partial void OnNotificationsEnabledChanged(bool value) => Preferences.Set("NotificationsEnabled", value);

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        var confirm = await Application.Current.MainPage.DisplayAlert("Account löschen", "Bist du sicher?", "Ja", "Nein");
        if (!confirm) return;

        await UserService.DeleteCurrentUserAsync();
        await NavigationService.NavigateToAsync(Routes.LoginPage);
    }

    [RelayCommand]
    private async Task ReturnAsync() => await NavigationService.GoBackAsync();
}