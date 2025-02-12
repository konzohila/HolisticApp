using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class AnamnesisViewModel : BaseViewModel
{
    [ObservableProperty] private string _age = string.Empty;
    [ObservableProperty] private string _selectedGender = string.Empty;
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private bool _hasComplaint;
    [ObservableProperty] private string _selectedComplaint = string.Empty;
    [ObservableProperty] private double _severity = 1;

    public string[] GenderOptions { get; } = ["Männlich", "Weiblich", "Divers"];
    public string[] ComplaintOptions { get; } = ["Verdauungsbeschwerden", "Kopfschmerzen", "Rückenschmerzen"];

    public AnamnesisViewModel(INavigationService navigationService, IUserService userService, ILogger<AnamnesisViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    public async Task InitializeAsync()
    {
        var user = await UserService.GetLoggedInUserAsync();
        if (user != null)
        {
            Age = user.Age?.ToString() ?? string.Empty;
            SelectedGender = string.IsNullOrEmpty(user.Gender) ? GenderOptions[0] : user.Gender;
            Height = user.Height?.ToString() ?? string.Empty;
            Weight = user.Weight?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(user.CurrentComplaint) && user.CurrentComplaint != "Keine Beschwerden")
            {
                HasComplaint = true;
                var parts = user.CurrentComplaint.Split(" (Stärke: ");
                SelectedComplaint = parts[0];

                if (parts.Length > 1 && parts[1].EndsWith("/10)"))
                {
                    var severityStr = parts[1].Replace("/10)", "");
                    if (double.TryParse(severityStr, out var parsedSeverity))
                        Severity = parsedSeverity;
                }
            }
        }
    }

    [RelayCommand]
    private async Task ReturnAsync() => await NavigationService.GoBackAsync();

    [RelayCommand]
    private async Task SaveAsync()
    {
        var user = await UserService.GetLoggedInUserAsync();
        if (user == null)
        {
            Logger.LogError("Kein eingeloggter Benutzer gefunden.");
            return;
        }

        user.Age = int.TryParse(Age, out var parsedAge) ? parsedAge : null;
        user.Gender = SelectedGender;
        user.Height = decimal.TryParse(Height, out var parsedHeight) ? parsedHeight : null;
        user.Weight = decimal.TryParse(Weight, out var parsedWeight) ? parsedWeight : null;
        user.CurrentComplaint = HasComplaint ? $"{SelectedComplaint} (Stärke: {Severity}/10)" : "Keine Beschwerden";

        var result = await UserService.UpdateUserAsync(user);
        if (result)
        {
            Logger.LogInformation("Anamnese erfolgreich gespeichert für Benutzer {UserId}", user.Id);
            await NavigationService.NavigateToAsync(Routes.HomePage);
        }
    }
}