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
                // Stelle sicher, dass Android.App.Application.Context nicht null ist, bevor darauf zugegriffen wird
                var externalFilesDir = Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
                if (string.IsNullOrEmpty(externalFilesDir))
                {
                    throw new InvalidOperationException("ExternalFilesDir konnte nicht ermittelt werden.");
                }

                var logDirectory = Path.Combine(externalFilesDir, "HolisticAppLogs");
                Directory.CreateDirectory(logDirectory); // Stellt sicher, dass der Ordner existiert
                var logFilePath = Path.Combine(logDirectory, "app.log");

                // Konfiguriere Serilog mit verbessertem Fehlerhandling
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
                // Falls das Logging fehlschlägt, verhindere einen Absturz und lasse das Programm fortfahren.
            }

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Lösche andere Logging Provider und füge Serilog als einzigen Provider hinzu
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Datenbankverbindung initialisieren (mit Absicherung gegen Nullverweise)
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

            builder.Services.AddTransient<ViewModels.LoginViewModel>();
            builder.Services.AddTransient<ViewModels.RegistrationViewModel>();
            builder.Services.AddTransient<ViewModels.HomeViewModel>();
            builder.Services.AddTransient<ViewModels.AnamnesisViewModel>();
            builder.Services.AddTransient<ViewModels.UserMenuViewModel>();
            builder.Services.AddTransient<ViewModels.DoctorDashboardViewModel>();
            builder.Services.AddTransient<ViewModels.DoctorRegistrationViewModel>();

            return builder.Build();
        }
    }
}