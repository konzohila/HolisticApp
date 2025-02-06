using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            // DI-Zugriff über den MauiContext
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            BindingContext = new LoginViewModel(userRepository, Navigation);
        }
    }
}