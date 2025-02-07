using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage(User currentUser)
        {
            InitializeComponent();
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            BindingContext = new AdminDashboardViewModel(currentUser, userRepository, Navigation);
        }
    }
}