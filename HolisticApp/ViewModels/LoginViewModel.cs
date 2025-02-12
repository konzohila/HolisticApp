using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Enums;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _emailOrUsername = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;
    private readonly IUserService _userService;

    public LoginViewModel(INavigationService navigationService, IUserService userService, ILogger<LoginViewModel> logger)
        : base(navigationService, userService, logger)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Fehler", "Bitte alle Felder ausfüllen!", "OK");
            return;
        }

        Logger.LogInformation("Login-Versuch für {EmailOrUsername}", EmailOrUsername);
        var result = await UserService.LoginAsync(EmailOrUsername, Password);

        switch (result.Status)
        {
            case LoginStatus.Success:
            {
                switch (result.User.Role)
                {
                    case UserRole.Admin:
                        await NavigationService.NavigateToAsync(Routes.AdminDashboardPage);
                        break;
                    case UserRole.Doctor:
                        await NavigationService.NavigateToAsync(Routes.DoctorDashboardPage);
                        break;
                    default:
                        var anamnesisCompleted = await _userService.IsAnamnesisCompletedAsync();
                        var targetPage = anamnesisCompleted ? Routes.HomePage : Routes.AnamnesisPage;
                        await NavigationService.NavigateToAsync(targetPage);
                        break;
                }
                break;
            }
            case LoginStatus.UserNotFound:
                await Application.Current.MainPage.DisplayAlert("Fehler", "Benutzer nicht gefunden!", "OK");
                break;
            case LoginStatus.InvalidPassword:
                await Application.Current.MainPage.DisplayAlert("Fehler", "Passwort inkorrekt!", "OK");
                break;
            case LoginStatus.Error:
                await Application.Current.MainPage.DisplayAlert("Fehler", "Beim Login ist ein Fehler aufgetreten!", "OK")!;
                break;
            }
        }
    }