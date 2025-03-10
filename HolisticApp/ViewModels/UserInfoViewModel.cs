using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        Age = user.Age.HasValue ? $"{user.Age} Jahre" : "Kein Alter angegeben";
        Gender = user.Gender ?? "Nicht angegeben";
        Height = user.Height.HasValue ? $"{user.Height} cm" : "Keine Größe angegeben";
        Weight = user.Weight.HasValue ? $"{user.Weight} kg" : "Kein Gewicht angegeben";
        Complaint = string.IsNullOrEmpty(user.CurrentComplaint) ? "Keine Beschwerden" : user.CurrentComplaint;
        /*if (user.MasterAccountId.HasValue)
            UserService.*/
        Doctor = user.MasterAccountId?.ToString() ?? "Kein behandelnder Therapeut angegeben";
    }

    [RelayCommand]
    private async Task ReturnAsync() => await NavigationService.GoBackAsync();
}