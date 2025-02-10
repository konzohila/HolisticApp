using System.Diagnostics;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;

namespace HolisticApp
{
    public partial class App
    {
        private readonly IUserRepository _userRepository;

        public App(IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new NavigationPage(new LoadingPage()));
            InitializeAsync(window);
            return window;
        }

        private async void InitializeAsync(Window window)
        {
            try
            {
                int userId = Preferences.Get("LoggedInUserId", 0);
                if (userId <= 0)
                {
                    Debug.WriteLine("[App] Kein Benutzer angemeldet. Navigiere zur Login-Seite.");
                    var services = Current?.Handler?.MauiContext?.Services;
                    if (services != null)
                    {
                        var loginPage = services.GetRequiredService<LoginPage>();
                        window.Page = loginPage;
                    }
                    return;
                }

                var user = await _userRepository.GetUserAsync(userId);
                if (user == null)
                {
                    Debug.WriteLine($"[App] Kein User für die gespeicherte ID ({userId}) gefunden. Navigiere zur Login-Seite.");
                    var services = Current?.Handler?.MauiContext?.Services;
                    if (services != null)
                    {
                        var loginPage = services.GetRequiredService<LoginPage>();
                        window.Page = loginPage;
                    }
                    return;
                }

                Page newPage;
                switch (user.Role)
                {
                    case UserRole.Doctor:
                        Debug.WriteLine($"[App] User {user.Id} (Doctor) gefunden. Navigiere zur Doktor-Dashboard-Seite.");
                        var services = Current?.Handler?.MauiContext?.Services;
                        if (services != null)
                        {
                            var doctorDashboardPage = services.GetRequiredService<DoctorDashboardPage>();
                            newPage = doctorDashboardPage;
                        }
                        break;
                    case UserRole.Admin:
                        Debug.WriteLine($"[App] User {user.Id} (Admin) gefunden. Navigiere zur Admin-Dashboard-Seite.");
                        if (Application.Current?.Handler != null)
                        {
                            var services = Application.Current.Handler.MauiContext?.Services;
                            if (services != null)
                            {
                                var adminDashboardPage = services.GetRequiredService<AdminDashboardPage>();
                                newPage = adminDashboardPage;
                            }
                        }
                        break;
                    default:
                        bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                        Debug.WriteLine($"[App] User {user.Id} (Patient) gefunden. Anamnese abgeschlossen: {anamnesisCompleted}");
                        newPage = anamnesisCompleted ? new HomePage(user) : new AnamnesisPage(user);
                        break;
                }
                window.Page = new NavigationPage(newPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Fehler während der Initialisierung: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                window.Page = new NavigationPage(new LoginPage());
            }
        }
    }
}