using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using HolisticApp.Constants;

namespace HolisticApp.ViewModels;

public partial class DoctorDashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<User> _patients = [];

    [ObservableProperty]
    private string _generatedInvitationLink = string.Empty;

    public DoctorDashboardViewModel(INavigationService navigationService, IUserService userService, ILogger<DoctorDashboardViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    public async Task InitializeAsync()
    {
        /*var user = await UserService.GetLoggedInUserAsync();
        if (user?.Role == UserRole.Doctor)
        {
            var patientList = await UserService.GetPatientsForDoctorAsync(user.Id);
            Patients = new ObservableCollection<User>(patientList);
        }*/
    }

    [RelayCommand]
    private async Task GenerateInvitationAsync()
    {
        /*var user = await UserService.GetLoggedInUserAsync();
        if (user?.Role != UserRole.Doctor) return;

        var invitationLink = await UserService.GenerateInvitationLinkAsync(user.Id);
        GeneratedInvitationLink = invitationLink;*/
    }

    [RelayCommand]
    private async Task OpenUserMenuAsync() => await NavigationService.NavigateToAsync(Routes.UserMenuPage);
}