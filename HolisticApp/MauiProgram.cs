using HolisticApp;
using HolisticApp.Data;
using HolisticApp.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HolisticApp
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ConnectionString – ggf. aus Konfiguration lesen
            string connectionString = "Server=database-1.cjs4qmoaa9sv.eu-central-1.rds.amazonaws.com;Database=holisticapp;User=admin;Password=pwpwpwpw;";
            builder.Services.AddSingleton<IUserRepository>(new UserRepository(connectionString));

            // Registrierung der ViewModels
            builder.Services.AddTransient<ViewModels.LoginViewModel>();
            builder.Services.AddTransient<ViewModels.RegistrationViewModel>();
            builder.Services.AddTransient<ViewModels.HomeViewModel>();
            builder.Services.AddTransient<ViewModels.AnamnesisViewModel>();
            builder.Services.AddTransient<ViewModels.UserMenuViewModel>();

            return builder.Build();
        }
    }
}