using HolisticApp.ViewModels;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HolisticApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                // Log-Datei einrichten
                var externalFilesDir = Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
                if (string.IsNullOrEmpty(externalFilesDir))
                {
                    throw new InvalidOperationException("ExternalFilesDir konnte nicht ermittelt werden.");
                }

                var logDirectory = Path.Combine(externalFilesDir, "HolisticAppLogs");
                Directory.CreateDirectory(logDirectory);
                var logFilePath = Path.Combine(logDirectory, "app.log");

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(
                        logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Initialisieren des Loggings: {ex}");
            }

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Logging konfigurieren
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // **Shell-Registrierung für die Navigation**
            builder.Services.AddSingleton<Shell>();

            // **Navigation registrieren**
            builder.Services.AddSingleton<INavigation>(sp =>
            {
                var shell = sp.GetRequiredService<Shell>();
                var navService = shell.Navigation;
                if (navService == null)
                {
                    throw new InvalidOperationException("INavigation konnte nicht aus Shell extrahiert werden.");
                }
                return navService;
            });

            // **Datenbankverbindung initialisieren**
            string connectionString = "Server=database-1.cjs4qmoaa9sv.eu-central-1.rds.amazonaws.com;Database=holisticapp;User=admin;Password=pwpwpwpw;";
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Log.Error("Die Verbindungszeichenfolge für die Datenbank ist leer oder null.");
            }
            else
            {
                builder.Services.AddSingleton<Data.Interfaces.IUserRepository>(sp =>
                    new Data.UserRepository(connectionString,
                        sp.GetRequiredService<ILogger<Data.UserRepository>>()));

                builder.Services.AddSingleton<Data.Interfaces.IInvitationRepository>(sp =>
                    new Data.InvitationRepository(connectionString,
                        sp.GetRequiredService<ILogger<Data.InvitationRepository>>()));
            }

            // **ViewModels mit DI für Logger & Navigation registrieren**
            builder.Services.AddTransient<ViewModels.LoginViewModel>(sp =>
                new ViewModels.LoginViewModel(
                    sp.GetRequiredService<Data.Interfaces.IUserRepository>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.LoginViewModel>>()),
                    sp.GetRequiredService<Views.AdminDashboardPage>();

            builder.Services.AddTransient<ViewModels.RegistrationViewModel>(sp =>
                new ViewModels.RegistrationViewModel(
                    sp.GetRequiredService<Data.Interfaces.IUserRepository>(),
                    sp.GetRequiredService<Data.Interfaces.IInvitationRepository>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.RegistrationViewModel>>()));

            builder.Services.AddTransient<ViewModels.HomeViewModel>(sp =>
                new ViewModels.HomeViewModel(
                    sp.GetRequiredService<Models.User>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.HomeViewModel>>()));

            builder.Services.AddTransient<ViewModels.AnamnesisViewModel>(sp =>
                new ViewModels.AnamnesisViewModel(
                    sp.GetRequiredService<Models.User>(),
                    sp.GetRequiredService<Data.Interfaces.IUserRepository>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.AnamnesisViewModel>>()));

            builder.Services.AddTransient<ViewModels.UserMenuViewModel>(sp =>
                new ViewModels.UserMenuViewModel(
                    sp.GetRequiredService<Models.User>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.UserMenuViewModel>>()));

            builder.Services.AddTransient<ViewModels.DoctorDashboardViewModel>(sp =>
                new ViewModels.DoctorDashboardViewModel(
                    sp.GetRequiredService<Models.User>(),
                    sp.GetRequiredService<Data.Interfaces.IUserRepository>(),
                    sp.GetRequiredService<Data.Interfaces.IInvitationRepository>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.DoctorDashboardViewModel>>()));

            builder.Services.AddTransient<ViewModels.DoctorRegistrationViewModel>(sp =>
                new ViewModels.DoctorRegistrationViewModel(
                    sp.GetRequiredService<Data.Interfaces.IUserRepository>(),
                    sp.GetRequiredService<INavigation>(),
                    sp.GetRequiredService<ILogger<ViewModels.DoctorRegistrationViewModel>>()));

            // **Pages registrieren**
            builder.Services.AddTransient<Views.LoadingPage>();
            builder.Services.AddTransient<Views.LoginPage>(sp =>
                new Views.LoginPage(
                    sp.GetRequiredService<LoginViewModel>()));
            builder.Services.AddTransient<Views.RegistrationPage>(sp =>
                new Views.RegistrationPage(
                    sp.GetRequiredService<RegistrationViewModel>()));
            builder.Services.AddTransient<Views.HomePage>(sp =>
                new Views.HomePage(
                    sp.GetRequiredService<HomeViewModel>()));
            builder.Services.AddTransient<Views.AnamnesisPage>(sp =>
                new Views.AnamnesisPage(
                    sp.GetRequiredService<AnamnesisViewModel>()));
            builder.Services.AddTransient<Views.UserMenuPage>(sp =>
                new Views.UserMenuPage(
                    sp.GetRequiredService<UserMenuViewModel>()));
            builder.Services.AddTransient<Views.AdminDashboardPage>(sp =>
                new Views.AdminDashboardPage(
                    sp.GetRequiredService<AdminDashboardViewModel>()));
            builder.Services.AddTransient<Views.DoctorDashboardPage>(sp =>
                new Views.DoctorDashboardPage(
                    sp.GetRequiredService<DoctorDashboardViewModel>()));
            builder.Services.AddTransient<Views.DoctorRegistrationPage>(sp =>
                new Views.DoctorRegistrationPage(
                    sp.GetRequiredService<DoctorRegistrationViewModel>()));
            builder.Services.AddTransient<Views.PatientDetailPage>();

            return builder.Build();
        }
    }
}