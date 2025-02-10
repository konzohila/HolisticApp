using Microsoft.Extensions.Logging;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class DoctorRegistrationPage
    {
        private readonly ILogger<DoctorRegistrationPage> _logger;
        
        public DoctorRegistrationPage(ILogger<DoctorRegistrationPage> logger)
        {
            InitializeComponent();
            _logger = logger;
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                int loggedInUserId = Preferences.Get("LoggedInUserId", 0);
                if (loggedInUserId == 0)
                {
                    _logger.LogWarning("Kein Benutzer angemeldet (UserID = 0).");
                    await DisplayAlert("Fehler", "Kein Benutzer angemeldet.", "OK");
                    await Navigation.PopAsync();
                    return;
                }

                if (Application.Current is not App app)
                {
                    throw new InvalidOperationException("Application ist nicht vom erwarteten Typ.");
                }

                var services = app.Handler?.MauiContext?.Services;
                if (services == null)
                {
                    throw new InvalidOperationException("DI-Services nicht verfügbar.");
                }
                
                var userRepository = services.GetService(typeof(IUserRepository)) as IUserRepository;
                if (userRepository == null)
                {
                    throw new InvalidOperationException("UserRepository nicht gefunden.");
                }

                _logger.LogInformation("Prüfe Rolle des aktuell eingeloggten Benutzers (ID: {UserID}).", loggedInUserId);
                var currentUser = await userRepository.GetUserAsync(loggedInUserId);

                if (currentUser == null)
                {
                    _logger.LogWarning("Kein Benutzer für die gespeicherte ID {UserID} gefunden.", loggedInUserId);
                    await DisplayAlert("Fehler", "Kein Benutzer gefunden.", "OK");
                    await Navigation.PopAsync();
                    return;
                }

                if (currentUser.Role != UserRole.Admin)
                {
                    _logger.LogWarning("Unberechtigter Zugriff: UserID {UserID} hat keine Admin-Rechte.", loggedInUserId);
                    await DisplayAlert("Unberechtigt", "Nur Administratoren können Doktoren registrieren.", "OK");
                    await Navigation.PopAsync();
                    return;
                }

                _logger.LogInformation("Benutzer (ID: {UserID}) hat Admin-Rechte. Initialisiere DoctorRegistrationViewModel.", loggedInUserId);
                BindingContext = new DoctorRegistrationViewModel(userRepository, Navigation);
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError(ioe, "Fehler (InvalidOperationException) in OnAppearing: {Message}", ioe.Message);
                await DisplayAlert("Fehler", "Ein interner Fehler ist aufgetreten. Bitte versuche es später erneut.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler in OnAppearing.");
                await DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}