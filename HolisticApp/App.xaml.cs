using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Storage;

namespace HolisticApp
{
    public partial class App : Application
    {
        private readonly IUserRepository _userRepository;

        public App(IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;
        }

        // Parameter als nullable deklariert
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Erstelle ein Fenster mit einer temporären Lade-Seite.
            var window = new Window(new NavigationPage(new LoadingPage()));
            // Starte asynchrone Initialisierung.
            InitializeAsync(window);
            return window;
        }

        private async void InitializeAsync(Window window)
        {
            var userId = Preferences.Get("LoggedInUserId", 0);
            Page newPage;
            if (userId > 0)
            {
                var user = await _userRepository.GetUserAsync(userId);
                if (user != null)
                {
                    if (user.Role == UserRole.Doctor)
                        newPage = new DoctorDashboardPage(user);
                    else if (user.Role == UserRole.Admin)
                        newPage = new AdminDashboardPage(user);
                    else
                    {
                        bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                        newPage = anamnesisCompleted ? new HomePage(user) : new AnamnesisPage(user);
                    }
                }
                else
                {
                    newPage = new LoginPage();
                }
            }
            else
            {
                newPage = new LoginPage();
            }

            // Aktualisiere die Page des Fensters.
            window.Page = new NavigationPage(newPage);
        }
    }
}