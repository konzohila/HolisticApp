1. Gesamtarchitektur der Anwendung

Die HolisticApp folgt einer 3-Schichten-Architektur mit dem MVVM-Pattern (Model-View-ViewModel). Dies sorgt für eine klare Trennung von UI, Geschäftslogik und Datenzugriff und ermöglicht eine gute Wartbarkeit, Testbarkeit und Skalierbarkeit.

2. Ablauf der Anwendung (Application Lifecycle)

Die folgende Grafik zeigt, wie die Anwendung startet und die Views aufgebaut werden.


```plaintext
┌──────────────────────────────┐
│      Anwendung startet       │
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│    App.xaml.cs (Factory)     │  
│ - Setzt DI-Container auf     │  
│ - Initialisiert Logging      │  
│ - Erstellt AppShell          │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│      MainPage = AppShell     │  
│ - Verwaltet Navigation       │  
│ - Definiert Hauptlayout      │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│    Views & ViewModels        │  
│ - ViewModels werden erstellt │  
│ - Datenbindung aktiv         │  
│ - Nutzerinteraktion startet  │  
└──────────────────────────────┘



3. Präsentationsschicht (UI Layer & MVVM)

Die UI-Schicht besteht aus Views (XAML), die mit ViewModels (C#-Klassen) interagieren. Sie sorgt für die Darstellung der App und verarbeitet Nutzereingaben. Die Kommunikation zwischen UI und Logik erfolgt über Binding und Commands.

3.1 ViewModels & ihre Rolle

ViewModels sind das Bindeglied zwischen UI und der Geschäftslogik. Sie enthalten keine UI-spezifische Logik, sondern:
	•	Bereiten Daten für die UI auf.
	•	Handhaben UI-Interaktionen (z. B. Button-Klicks).
	•	Rufen Services auf, um Daten zu verarbeiten.
	•	Halten die UI synchron mit dem Datenmodell (Model).

3.2 Ablauf eines Login-Buttons (Schaubild)

Ein Button-Klick auf “Login” folgt diesem Datenfluss:

┌──────────────────────────────┐
│      Nutzer klickt Login     │  (UI)
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│   XAML ruft Command auf      │  
│ - {Binding LoginCommand}     │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│  LoginViewModel (Schicht 1)  │  
│ - Führt Validierung durch    │  
│ - Ruft UserService auf       │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│  UserService (Schicht 2)     │  
│ - Prüft Benutzerdaten        │  
│ - Ruft UserRepository auf    │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│  UserRepository (Schicht 3)  │  
│ - Fragt Datenbank ab         │  
│ - Gibt Ergebnis zurück       │  
└──────────────────────────────┘
            │  
            ▼
┌──────────────────────────────┐
│  Daten zurück zum UI         │  
│ - ViewModel aktualisiert UI  │  
│ - Navigation zur HomePage    │  
└──────────────────────────────┘


3.3 Code Beispiel für LoginViewModel

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _emailOrUsername = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    public LoginViewModel(INavigationService navigationService, IUserService userService, ILogger<LoginViewModel> logger)
        : base(navigationService, userService, logger)
    {
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Fehler", "Bitte alle Felder ausfüllen!", "OK");
            return;
        }

        Logger.LogInformation("Login-Versuch für {EmailOrUsername}", EmailOrUsername);
        var result = await UserService.LoginAsync(EmailOrUsername, Password);

        switch (result.Status)
        {
            case LoginStatus.Success:
                await NavigationService.NavigateToAsync(Routes.HomePage);
                break;
            case LoginStatus.UserNotFound:
                await Application.Current.MainPage.DisplayAlert("Fehler", "Benutzer nicht gefunden!", "OK");
                break;
            case LoginStatus.InvalidPassword:
                await Application.Current.MainPage.DisplayAlert("Fehler", "Passwort inkorrekt!", "OK");
                break;
        }
    }
}

4. Geschäftslogikschicht (Business Logic Layer - BLL)

Die BLL-Schicht verarbeitet alle Geschäftsregeln und hält die App von der Datenbank unabhängig.

4.1 UserService – Verarbeitung von Logik

Beispiel für die Authentifizierung eines Benutzers:

public async Task<LoginResult> LoginAsync(string emailOrUsername, string password)
{
    var result = await _userRepository.AuthenticateUser(emailOrUsername, password);
    if (result.IsAuthenticated)
    {
        return new LoginResult(result.User, LoginStatus.Success);
    }
    return new LoginResult(null, LoginStatus.InvalidPassword);
}

5. Datenzugriffsschicht (Data Access Layer - DAL)

Die DAL-Schicht kommuniziert mit der Datenbank und führt CRUD-Operationen durch.

5.1 UserRepository – Datenbankzugriff

public async Task<User?> GetUserByEmailAsync(string email)
{
    await using var connection = await GetConnectionAsync();
    await using var command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM Users WHERE Email = @email";
    command.Parameters.AddWithValue("@email", email);

    await using var reader = await command.ExecuteReaderAsync();
    return await reader.ReadAsync() ? CreateUserFromReader(reader) : null;
}

Hier wird sichergestellt, dass asynchron auf die Datenbank zugegriffen wird.

6. Factory Pattern in App.xaml.cs

Das Factory Pattern wird in MauiProgram.cs genutzt, um Instanzen bereitzustellen.

6.1 Erklärung

Das Factory Pattern wird verwendet, um eine zentrale DI-Konfiguration zu schaffen. Alle Abhängigkeiten werden in einer einzigen Methode registriert.

6.2 Implementierung

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Dependency Injection
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        return builder.Build();
    }
}

Dadurch wird lose Kopplung erreicht.
