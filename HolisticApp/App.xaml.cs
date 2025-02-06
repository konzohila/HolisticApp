using HolisticApp.Data;
using HolisticApp.Views;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace HolisticApp
{
    public partial class App : Application
    {
        public static UserDatabase UserDatabase { get; private set; }

        public App()
        {
            InitializeComponent();

            // Setze eine temporäre MainPage, um den Fehler zu vermeiden.
            MainPage = new ContentPage { Content = new Label { Text = "Lade..." } };

            // Initialisiere die Datenbank (Passe den ConnectionString entsprechend an)
            UserDatabase = new UserDatabase("Server=10.0.2.2;Database=holisticapp;User=root;Password=;");

            // Starte die asynchrone Initialisierung
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            var userId = Preferences.Get("LoggedInUserId", 0);
            if (userId > 0)
            {
                var user = await UserDatabase.GetUserAsync(userId);
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