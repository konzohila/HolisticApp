using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace HolisticApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                // Stelle sicher, dass Android.App.Application.Context nicht null ist, bevor darauf zugegriffen wird
                var externalFilesDir = Android.App.Application.Context?.GetExternalFilesDir(null)?.AbsolutePath;
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
                builder.Services.AddSingleton<HolisticApp.Data.Interfaces.IUserRepository>(sp =>
                    new HolisticApp.Data.UserRepository(connectionString,
                        sp.GetRequiredService<ILogger<HolisticApp.Data.UserRepository>>()));

                builder.Services.AddSingleton<HolisticApp.Data.Interfaces.IInvitationRepository>(sp =>
                    new HolisticApp.Data.InvitationRepository(connectionString,
                        sp.GetRequiredService<ILogger<HolisticApp.Data.InvitationRepository>>()));
            }

            builder.Services.AddTransient<HolisticApp.ViewModels.LoginViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.RegistrationViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.HomeViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.AnamnesisViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.UserMenuViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.DoctorDashboardViewModel>();
            builder.Services.AddTransient<HolisticApp.ViewModels.DoctorRegistrationViewModel>();

            return builder.Build();
        }
    }
}