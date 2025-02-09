using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using HolisticApp.Services;
using HolisticApp.Data;
using HolisticApp.Data.Interfaces;

namespace HolisticApp;

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


        var awsSecretsService = new AwsSecretsService();
        JObject secrets = Task.Run(async () => await awsSecretsService.GetSecretAsync("prod/AppBeta/Mysql")).Result;
        
        string username = secrets["username"]?.ToString();
        string password = secrets["password"]?.ToString();
        string host = secrets["host"]?.ToString();
        int port = secrets["port"]?.ToObject<int>() ?? 3306;

        string connectionString = $"Server={host};Port={port};User Id={username};Password={password};Database=database-1";

        // 🔹 Korrekte DI-Registrierung mit Factory-Methode
        builder.Services.AddSingleton<IUserRepository>(provider => new UserRepository(connectionString));

        return builder.Build();
    }
}