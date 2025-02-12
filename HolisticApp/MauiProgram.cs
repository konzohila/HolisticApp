using HolisticApp.Data.Interfaces;
using HolisticApp.Services.Interfaces;
using HolisticApp.ViewModels;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HolisticApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("app.log", rollingInterval: RollingInterval.Day)
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

        // **DI-Registrierungen**
        builder.Services.AddSingleton<INavigationService, Services.NavigationService>();
        builder.Services.AddSingleton<IUserService, Services.UserService>();

        // **ViewModels**
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AnamnesisViewModel>();
        builder.Services.AddTransient<UserMenuViewModel>();
        builder.Services.AddTransient<DoctorDashboardViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<UserInfoViewModel>();

        // **Views**
        builder.Services.AddTransient<Views.LoginPage>();
        builder.Services.AddTransient<Views.RegistrationPage>();
        builder.Services.AddTransient<Views.HomePage>();
        builder.Services.AddTransient<Views.AnamnesisPage>();
        builder.Services.AddTransient<Views.UserMenuPage>();
        builder.Services.AddTransient<Views.DoctorDashboardPage>();
        builder.Services.AddTransient<Views.SettingsPage>();
        builder.Services.AddTransient<Views.UserInfoPage>();

        return builder.Build();
    }
}