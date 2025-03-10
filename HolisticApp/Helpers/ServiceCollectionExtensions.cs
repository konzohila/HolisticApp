using Microsoft.Extensions.Logging;
using HolisticApp.Services.Interfaces;

namespace HolisticApp.Helpers
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registriert ein ViewModel mit NavigationService, UserService und Logger automatisch.
        /// </summary>
        public static IServiceCollection AddViewModel<TViewModel>(this IServiceCollection services)
            where TViewModel : class
        {
            services.AddTransient<TViewModel>(sp =>
            {
                var navigationService = sp.GetRequiredService<INavigationService>();
                var userService = sp.GetRequiredService<IUserService>();
                var logger = sp.GetRequiredService<ILogger<TViewModel>>();

                // Erstelle das ViewModel und übergebe die Standard-Parameter.
                return Activator.CreateInstance(typeof(TViewModel), navigationService, userService, logger) as TViewModel
                       ?? throw new InvalidOperationException($"Fehler beim Erstellen von {typeof(TViewModel).Name}");
            });

            return services;
        }
    }
}