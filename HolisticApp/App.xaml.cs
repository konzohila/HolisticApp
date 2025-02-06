using HolisticApp.Data.Interfaces;
using HolisticApp.Views;
using Microsoft.Maui.Storage;

namespace HolisticApp
{
    public partial class App : Application
    {
        public App(IUserRepository userRepository)
        {
            InitializeComponent();

            // Zeige zunächst eine Lade-Seite
            MainPage = new NavigationPage(new LoadingPage());

            // Starte asynchrone Initialisierung
            InitializeAsync(userRepository);
        }

        private async void InitializeAsync(IUserRepository userRepository)
        {
            var userId = Preferences.Get("LoggedInUserId", 0);
            if (userId > 0)
            {
                var user = await userRepository.GetUserAsync(userId);
                bool anamnesisCompleted = Preferences.Get($"AnamnesisCompleted_{user.Id}", false);
                MainPage = new NavigationPage(anamnesisCompleted ? new HomePage(user) : new AnamnesisPage(user));
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}