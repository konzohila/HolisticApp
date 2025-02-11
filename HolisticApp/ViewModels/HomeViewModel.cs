using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<HomeViewModel> _logger;
    [ObservableProperty]
    private string _userInitials = string.Empty;
    private User CurrentUser { get; }

    public HomeViewModel(User user,
        INavigationService navigationService,
        ILogger<HomeViewModel> logger)
    {
        CurrentUser = user;
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialisierung
        UserInitials = !string.IsNullOrWhiteSpace(user.Username)
            ? user.Username[..1].ToUpper()
            : string.Empty;
    }

    [RelayCommand]
    public async Task OpenAnamnesisAsync()
    {
        try
        {
            _logger.LogInformation("Öffne AnamnesisPage für User {UserId}", CurrentUser.Id);
            await _navigationService.NavigateToAsync("///AnamnesisPage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Öffnen der AnamnesisPage für User {UserId}", CurrentUser.Id);
        }
    }

    [RelayCommand]
    public async Task OpenUserMenuAsync()
    {
        try
        {
            _logger.LogInformation("Öffne UserMenuPage für User {UserId}", CurrentUser.Id);
            await _navigationService.NavigateToAsync("///UserMenuPage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für User {UserId}", CurrentUser.Id);
        }
    }
}