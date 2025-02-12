using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using HolisticApp.Models;

namespace HolisticApp.ViewModels;

public partial class RegistrationViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _invitationToken = string.Empty;

    public RegistrationViewModel(INavigationService navigationService, IUserService userService, ILogger<RegistrationViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Fehler", "Bitte alle Felder ausfüllen!", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(InvitationToken))
        {
            /*var isValid = await UserService.ValidateInvitationAsync(InvitationToken);
            if (!isValid)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Ungültiger oder abgelaufener Einladungscode!", "OK");
                return;
            }

            user.MasterAccountId = await UserService.GetMasterAccountIdFromInvitationAsync(InvitationToken);*/
        }

        var result = await UserService.RegisterUserAsync(Username, Email, Password);
        if (result)
        {
            await Application.Current.MainPage.DisplayAlert("Erfolg", "Registrierung erfolgreich!", "OK");
            await NavigationService.NavigateToAsync(Routes.LoginPage);
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Fehler", "Registrierung fehlgeschlagen!", "OK");
        }
    }

    [RelayCommand]
    private async Task ReturnAsync() => await NavigationService.GoBackAsync();
}