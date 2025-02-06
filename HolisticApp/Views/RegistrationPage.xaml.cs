using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            BindingContext = new RegistrationViewModel(userRepository, Navigation);
        }
    }
}