📌 HolisticApp – Architekturübersicht

Diese Anwendung basiert auf einer 3-Schichten-Architektur und folgt dem MVVM-Muster (Model-View-ViewModel). Sie nutzt Dependency Injection (DI), das Factory-Pattern zur vereinfachten Bereitstellung von ViewModels und das Repository-Pattern für den Zugriff auf die Datenbank.


1️⃣ Die 3-Schichten-Architektur

Die Anwendung ist in drei klare Schichten unterteilt, um eine saubere Trennung der Verantwortlichkeiten (Separation of Concerns) zu gewährleisten:

🔹 Schicht 1 – Präsentationsschicht (UI & ViewModels)

➡ Was passiert hier?
    •    Diese Schicht umfasst die XAML-Views (Benutzeroberfläche) und die dazugehörigen ViewModels, die für die Datenbindung und Geschäftslogik der UI zuständig sind.
    •    ViewModels greifen NICHT direkt auf die Datenbank zu! Sie interagieren nur mit den Services der zweiten Schicht.

➡ Wer nutzt was?
    •    Views (XAML) nutzen nur ihre ViewModels.
    •    ViewModels nutzen ausschließlich Services aus Schicht 2.

➡ Regeln:
✅ Views dürfen nur das ViewModel als BindingContext setzen.
✅ Kein Code-Behind außer InitializeComponent() in den Views!
✅ Keine direkte Datenbankabfragen oder Geschäftslogik in der UI!

🔹 Schicht 2 – Service-Schicht (Geschäftslogik)

➡ Was passiert hier?
    •    Diese Schicht verwaltet die gesamte Geschäftslogik der App.
    •    Services wie UserService verwalten Benutzeroperationen (Login, Logout, Benutzer laden/löschen).
    •    Sie greifen nur über Repository-Schnittstellen (IUserRepository) auf die Datenbank zu.

➡ Wer nutzt was?
    •    ViewModels nutzen Services.
    •    Services nutzen nur Repositories (Schicht 3).

➡ Regeln:
✅ Services verwalten ALLE Geschäftslogik.
✅ Services kapseln den Zugriff auf Repositories.
✅ ViewModels dürfen keine Datenbankzugriffe durchführen!

🔹 Schicht 3 – Datenzugriffsschicht (Repositories & Models)

➡ Was passiert hier?
    •    Diese Schicht kapselt den direkten Zugriff auf die Datenbank.
    •    Das Repository-Pattern wird verwendet, um eine lose Kopplung zu gewährleisten.
    •    Die UserRepository-Klasse kommuniziert mit der Datenbank und liefert User-Objekte zurück.

➡ Wer nutzt was?
    •    Services greifen auf Repositories zu.
    •    Repositories arbeiten mit der Datenbank und geben Models zurück.

➡ Regeln:
✅ Repositories sind die einzige Schicht, die direkten Datenbankzugriff hat.
✅ Repositories sollten keine Geschäftslogik enthalten.
✅ Repositories liefern nur Datenmodelle zurück.


2️⃣ Factory-Pattern & Dependency Injection (DI)

In der MauiProgram.cs werden alle Services, Repositories und ViewModels automatisch über DI registriert.

🔹 Factory-Pattern für ViewModels

Wir nutzen eine Factory-Methode, um ViewModels automatisch mit NavigationService, UserService und Logger zu versorgen.

📌 Vorteile des Factory-Patterns in DI:
✅ Kein Boilerplate-Code mehr für DI in jedem ViewModel.
✅ ViewModels müssen ihre Abhängigkeiten nicht selbst instanziieren.
✅ Änderungen in der Service-Struktur müssen nur an einer Stelle erfolgen.

📌 Beispiel:
📍 Datei: Helpers/ServiceCollectionExtensions.cs
public static IServiceCollection AddViewModel<TViewModel>(this IServiceCollection services)
    where TViewModel : class
{
    services.AddTransient<TViewModel>(sp =>
    {
        var navigationService = sp.GetRequiredService<INavigationService>();
        var userService = sp.GetRequiredService<IUserService>();
        var logger = sp.GetRequiredService<ILogger<TViewModel>>();

        return Activator.CreateInstance(typeof(TViewModel), navigationService, userService, logger) as TViewModel
            ?? throw new InvalidOperationException($"Fehler beim Erstellen von {typeof(TViewModel).Name}");
    });

    return services;
}

📍 MauiProgram.cs – Registrieren der ViewModels mit Factory-Methode
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder.Services.AddSingleton<IUserRepository, UserRepository>();
    builder.Services.AddSingleton<IUserService, UserService>();
    builder.Services.AddSingleton<INavigationService, NavigationService>();
    builder.Services.AddSingleton<SessionUser>();

    // Automatische DI für ViewModels
    builder.Services.AddViewModel<LoginViewModel>();
    builder.Services.AddViewModel<HomeViewModel>();
    builder.Services.AddViewModel<SettingsViewModel>();

    return builder.Build();
}


3️⃣ Repository-Pattern im UserRepository

📌 Warum Repository-Pattern?
✅ Trennung von Geschäftslogik und Datenbankzugriff.
✅ Erleichtert das Testen (z. B. durch Mocking von IUserRepository).
✅ Erlaubt den späteren Wechsel der Datenquelle ohne Änderungen in der Service-Schicht.

📍 Datei: Data/UserRepository.cs
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(string connectionString, ILogger<UserRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT * FROM Users WHERE Id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    Username = reader.GetString("Username")
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen des Benutzers mit ID {UserId}", id);
        }
        return null;
    }
}


4️⃣ MVVM: Klare Trennung zwischen UI, Logik und Daten

📌 Warum MVVM?
✅ Saubere Trennung von UI, Logik und Daten.
✅ Vermeidung von Code-Behind in den Views.
✅ Bessere Testbarkeit (ViewModels sind unabhängig von UI).

📍 Beispiel für eine XAML-View
<ContentPage 
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:viewmodels="clr-namespace:HolisticApp.ViewModels"
    x:Class="HolisticApp.Views.LoginPage"
    x:DataType="viewmodels:LoginViewModel"
    Title="Login">
    
    <Entry Text="{Binding EmailOrUsername}" Placeholder="Email oder Benutzername" />
    <Entry Text="{Binding Password}" IsPassword="True" Placeholder="Passwort" />
    <Button Text="Login" Command="{Binding LoginCommand}" />
</ContentPage>

📍 Code-Behind (LoginPage.xaml.cs)
public partial class LoginPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

✅ Kein Code-Behind außer InitializeComponent() und Setzen des BindingContext!


5️⃣ Vererbung von BaseViewModel

📌 Warum?
✅ Vermeidet redundanten Code in allen ViewModels.
✅ Erleichtert die Nutzung von Standard-Diensten (Navigation, Logging).

📍 BaseViewModel.cs
public abstract class BaseViewModel : ObservableObject
{
    protected readonly INavigationService NavigationService;
    protected readonly IUserService UserService;
    protected readonly ILogger Logger;

    protected BaseViewModel(INavigationService navigationService, IUserService userService, ILogger logger)
    {
        NavigationService = navigationService;
        UserService = userService;
        Logger = logger;
    }
}
