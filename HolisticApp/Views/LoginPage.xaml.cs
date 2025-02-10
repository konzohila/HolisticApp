using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Views
{
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext == null)
            {
                var app = Application.Current as App 
                          ?? throw new InvalidOperationException("Die Application ist nicht vom erwarteten Typ.");
                var services = app.Handler?.MauiContext?.Services 
                               ?? throw new InvalidOperationException("DI-Services nicht verfügbar.");
                var userRepository = services.GetService(typeof(IUserRepository)) as IUserRepository 
                                     ?? throw new InvalidOperationException("UserRepository nicht gefunden.");
                var logger = services.GetService(typeof(ILogger<LoginViewModel>)) as ILogger<LoginViewModel> 
                             ?? throw new InvalidOperationException("Logger für LoginViewModel nicht gefunden.");
                BindingContext = new LoginViewModel(userRepository, Navigation, logger);
            }
        }
    }
}