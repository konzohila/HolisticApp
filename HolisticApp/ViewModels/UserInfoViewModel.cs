using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class UserInfoViewModel : BaseViewModel
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _age = string.Empty;
    [ObservableProperty] private string _gender = string.Empty;
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private string _complaint = string.Empty;
    [ObservableProperty] private string _doctor = string.Empty;

    public UserInfoViewModel(INavigationService navigationService, IUserService userService, ILogger<UserInfoViewModel> logger)
        : base(navigationService, userService, logger)
    {
        LoadUserData();
    }

    private async void LoadUserData()
    {
        var user = await UserService.GetLoggedInUserAsync();
        if (user == null) return;

        Username = user.Username;
        Email = user.Email;
        Age = user.Age?.ToString() ?? "Nicht angegeben";
        Gender = user.Gender ?? "Nicht angegeben";
        Height = user.Height?.ToString() ?? "Nicht angegeben";
        Weight = user.Weight?.ToString() ?? "Nicht angegeben";
        Complaint = string.IsNullOrEmpty(user.CurrentComplaint) ? "Keine Beschwerden" : user.CurrentComplaint;
        Doctor = user.MasterAccountId?.ToString() ?? "Unbekannt";
    }

    [RelayCommand]
    private async Task ReturnAsync() => await NavigationService.GoBackAsync();
}