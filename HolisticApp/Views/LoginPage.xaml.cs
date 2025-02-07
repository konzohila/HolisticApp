using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            // Hier NICHT auf den DI-Container zugreifen, da Handler noch nicht initialisiert ist.
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        
            // Prüfen, ob bereits ein BindingContext gesetzt wurde
            if (BindingContext == null)
            {
                // Jetzt ist der Handler verfügbar:
                var userRepository = (Application.Current as App)
                    .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            
                // Setze den BindingContext mit dem LoginViewModel
                BindingContext = new LoginViewModel(userRepository, Navigation);
            }
        }
    }
}