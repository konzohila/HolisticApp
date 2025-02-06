using HolisticApp.Data;
using HolisticApp.Views;

namespace HolisticApp
{
    public partial class App : Application
    {
        public static UserDatabase UserDatabase { get; private set; }

        public App()
        {
            InitializeComponent();

            // Initialisiere die Datenbank (Passe den ConnectionString entsprechend an)
            UserDatabase = new UserDatabase("Server=10.0.2.2;Database=holisticapp;User=root;Password=;");

            // Prüfe, ob bereits ein Benutzer eingeloggt ist
            if (Preferences.ContainsKey("LoggedInUserId"))
            {
                // Hier kannst du ggf. anhand der gespeicherten UserId den User aus der DB laden.
                // Für den einfachen Fall: direkt zur HomePage navigieren.
                // Optional: prüfe auch das Flag "AnamnesisCompleted"
                bool anamnesisCompleted = Preferences.Get("AnamnesisCompleted", false);
                MainPage = new NavigationPage(anamnesisCompleted ? new HomePage(null) : new AnamnesisPage(null));
            }
            else
            {
                // Kein Benutzer eingeloggt, also LoginPage als Startseite
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}