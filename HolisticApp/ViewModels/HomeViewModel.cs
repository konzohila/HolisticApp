using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ILogger<HomeViewModel> _logger;
    private readonly IUserSession _userSession;
    [ObservableProperty]
    private string _userInitials = string.Empty;

    public HomeViewModel(INavigationService navigationService,
        ILogger<HomeViewModel> logger, IUserSession userSession)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userSession = userSession;

        // Initialisierung
        UserInitials = !string.IsNullOrWhiteSpace(_userSession.CurrentUser.Username)
            ? _userSession.CurrentUser.Username[..1].ToUpper()
            : string.Empty;
    }

    [RelayCommand]
    public async Task OpenAnamnesisAsync()
    {
        try
        {
            _logger.LogInformation("Öffne AnamnesisPage für User {UserId}", _userSession.CurrentUser.Id);
            await _navigationService.NavigateToAsync("///AnamnesisPage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Öffnen der AnamnesisPage für User {UserId}", _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    public async Task OpenUserMenuAsync()
    {
        try
        {
            _logger.LogInformation("Öffne UserMenuPage für User {UserId}", _userSession.CurrentUser.Id);
            await _navigationService.NavigateToAsync(Routes.UserMenuPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für User {UserId}", _userSession.CurrentUser.Id);
        }
    }
}