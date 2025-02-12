using CommunityToolkit.Mvvm.ComponentModel;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected readonly INavigationService NavigationService;
        protected readonly IUserService UserService;
        protected readonly ILogger Logger;

        protected BaseViewModel(INavigationService navigationService, IUserService userService, ILogger logger)
        {
            NavigationService = navigationService;
            UserService = userService;
            Logger = logger;
        }
    }
}