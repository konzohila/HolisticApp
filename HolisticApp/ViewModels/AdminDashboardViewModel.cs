using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using HolisticApp.Constants;

namespace HolisticApp.ViewModels;

public partial class AdminDashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<User> _doctors = [];

    public AdminDashboardViewModel(INavigationService navigationService, IUserService userService, ILogger<AdminDashboardViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    public async Task InitializeAsync()
    {
        var doctorList = await UserService.FindUsersByRole(UserRole.Doctor);
        Doctors = new ObservableCollection<User>(doctorList);
    }

    [RelayCommand]
    private async Task CreateDoctorAsync() => await NavigationService.NavigateToAsync(Routes.DoctorRegistrationPage);

    [RelayCommand]
    private async Task OpenUserMenuAsync() => await NavigationService.NavigateToAsync(Routes.UserMenuPage);
}