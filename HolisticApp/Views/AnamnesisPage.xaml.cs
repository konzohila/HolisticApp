using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class AnamnesisPage : ContentPage
    {
        public AnamnesisPage(User user)
        {
            InitializeComponent();
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            BindingContext = new AnamnesisViewModel(user, userRepository, Navigation);
        }
    }
}