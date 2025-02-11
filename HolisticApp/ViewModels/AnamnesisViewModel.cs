using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Data.Interfaces;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class AnamnesisViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;
    private readonly INavigationService _navigationService;
    private readonly ILogger<AnamnesisViewModel> _logger;
    private readonly IUserSession _userSession;
    
    [ObservableProperty]
    private string _age = string.Empty;

    [ObservableProperty]
    private string _selectedGender = string.Empty;

    [ObservableProperty]
    private string _height = string.Empty;

    [ObservableProperty]
    private string _weight = string.Empty;

    [ObservableProperty]
    private bool _hasComplaint;

    [ObservableProperty]
    private string _selectedComplaint = string.Empty;

    [ObservableProperty]
    private double _severity = 1;

    public string[] GenderOptions { get; } = ["Männlich", "Weiblich", "Divers"];
    public string[] ComplaintOptions { get; } = ["Verdauungsbeschwerden", "Kopfschmerzen", "Rückenschmerzen"];

    public AnamnesisViewModel(IUserRepository userRepository,
        INavigationService navigationService,
        ILogger<AnamnesisViewModel> logger, IUserSession userSession)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userSession = userSession;

        // Fülle die Felder mit aktuellen User-Daten
        if (_userSession.CurrentUser is { Age: not null }) _age = _userSession.CurrentUser.Age.Value.ToString();
        SelectedGender = string.IsNullOrEmpty(_userSession.CurrentUser?.Gender) ? GenderOptions[0] : _userSession.CurrentUser.Gender;
        if (_userSession.CurrentUser != null)
        {
            Height = _userSession.CurrentUser.Height?.ToString() ?? string.Empty;
            Weight = _userSession.CurrentUser.Weight?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(_userSession.CurrentUser.CurrentComplaint) &&
                _userSession.CurrentUser.CurrentComplaint != "Keine Beschwerden")
            {
                _hasComplaint = true;
                var parts = _userSession.CurrentUser.CurrentComplaint.Split(" (Stärke: ");
                SelectedComplaint = parts[0];
                if (parts.Length > 1 && parts[1].EndsWith("/10)"))
                {
                    var severityStr = parts[1].Replace("/10)", "");
                    if (double.TryParse(severityStr, out var parsedSeverity))
                        _severity = parsedSeverity;
                }
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var currentPage = Application.Current?.Windows[0].Page;
        try
        {
            if (int.TryParse(Age, out var parsedAge))
            {
                if (_userSession.CurrentUser != null) _userSession.CurrentUser.Age = parsedAge;
            }
            else if (_userSession.CurrentUser != null) _userSession.CurrentUser.Age = null;

            if (_userSession.CurrentUser != null)
            {
                _userSession.CurrentUser.Gender = SelectedGender;

                if (decimal.TryParse(Height, out var parsedHeight))
                    _userSession.CurrentUser.Height = parsedHeight;
                else
                    _userSession.CurrentUser.Height = null;

                if (decimal.TryParse(Weight, out var parsedWeight))
                    _userSession.CurrentUser.Weight = parsedWeight;
                else
                    _userSession.CurrentUser.Weight = null;

                if (HasComplaint)
                {
                    if (string.IsNullOrEmpty(SelectedComplaint))
                    {
                        if (currentPage != null)
                            await currentPage.DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
                        _logger.LogWarning("Anamnese konnte nicht gespeichert werden (keine Beschwerde ausgewählt).");
                        return;
                    }

                    _userSession.CurrentUser.CurrentComplaint = $"{SelectedComplaint} (Stärke: {Severity}/10)";
                }
                else
                {
                    _userSession.CurrentUser.CurrentComplaint = "Keine Beschwerden";
                }

                _logger.LogInformation("Speichere Anamnese-Infos für User {UserId}", _userSession.CurrentUser.Id);
                var result = await _userRepository.SaveUserAsync(_userSession.CurrentUser);
                if (result > 0)
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                    Preferences.Set($"AnamnesisCompleted_{_userSession.CurrentUser.Id}", true);

                    _logger.LogInformation("Anamnese erfolgreich gespeichert für User {UserId}. Navigiere HomePage.",
                        _userSession.CurrentUser.Id);
                            await _navigationService.NavigateToAsync(Routes.HomePage);
                }
                else
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
                    _logger.LogError("Anamnese: Fehler beim Speichern in DB für User {UserId}.",
                        _userSession.CurrentUser.Id);
                }
            }
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Anamnese für User {UserId}.",
                    _userSession.CurrentUser.Id);
            if (currentPage != null)
                await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
        }
    }
}