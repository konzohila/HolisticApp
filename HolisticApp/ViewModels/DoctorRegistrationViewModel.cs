using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class DoctorRegistrationViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;
    private readonly INavigationService _navigationService;
    private readonly ILogger<DoctorRegistrationViewModel> _logger;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    public DoctorRegistrationViewModel(IUserRepository userRepository,
        INavigationService navigationService,
        ILogger<DoctorRegistrationViewModel> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        var currentPage = Application.Current?.Windows?[0]?.Page;
        try
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                _logger.LogWarning("DoctorRegistration fehlgeschlagen: Ein oder mehrere Felder sind leer.");
                return;
            }

            var doctor = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password, // In Produktion: PW hashen!
                Role = UserRole.Doctor,
                MasterAccountId = null
            };

            _logger.LogInformation("Versuche, Doktor {Name} zu registrieren.", Username);
            var result = await _userRepository.SaveUserAsync(doctor);
            if (result > 0)
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Erfolg", "Doktor erfolgreich registriert.", "OK");
                _logger.LogInformation("Doktor {Name} wurde erfolgreich registriert.", Username);
                await _navigationService.GoBackAsync();
            }
            else
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Fehler beim Registrieren des Doktors.", "OK");
                _logger.LogError("Fehler beim Registrieren von Doktor {Name}.", Username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unerwarteter Fehler bei der Doktor-Registrierung für {Name}.", Username);
            if (currentPage != null)
                await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
        }
    }
}