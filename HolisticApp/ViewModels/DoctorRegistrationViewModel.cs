using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Constants;
using HolisticApp.Models;
using HolisticApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels;

public partial class DoctorRegistrationViewModel : BaseViewModel
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    public DoctorRegistrationViewModel(INavigationService navigationService, IUserService userService, ILogger<DoctorRegistrationViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        try
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await currentPage?.DisplayAlert("Fehler", "Bitte alle Felder ausfüllen.", "OK")!;
                Logger.LogWarning("DoctorRegistration fehlgeschlagen: Ein oder mehrere Felder sind leer.");
                return;
            }
            
            Logger.LogInformation("Versuche, Doktor {Name} zu registrieren.", Username);
            var result = await UserService.RegisterUserAsync(Username, Email, Password);

            if (result)
            {
                await currentPage?.DisplayAlert("Erfolg", "Doktor erfolgreich registriert.", "OK")!;
                Logger.LogInformation("Doktor {Name} wurde erfolgreich registriert.", Username);
                await NavigationService.GoBackAsync();
            }
            else
            {
                await currentPage?.DisplayAlert("Fehler", "Fehler beim Registrieren des Doktors.", "OK")!;
                Logger.LogError("Fehler beim Registrieren von Doktor {Name}.", Username);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unerwarteter Fehler bei der Doktor-Registrierung für {Name}.", Username);
            await currentPage?.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK")!;
        }
    }
}