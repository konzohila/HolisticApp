using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class AdminDashboardPage
    {
        public AdminDashboardPage(User currentUser)
        {
            InitializeComponent();
            var app = Application.Current as App 
                      ?? throw new InvalidOperationException("Application ist nicht vom erwarteten Typ.");
            var services = app.Handler?.MauiContext?.Services 
                           ?? throw new InvalidOperationException("DI-Services nicht verfügbar.");
            var userRepository = services.GetService(typeof(IUserRepository)) as IUserRepository 
                                 ?? throw new InvalidOperationException("UserRepository nicht gefunden.");
            BindingContext = new AdminDashboardViewModel(currentUser, userRepository, Navigation);
        }
    }
}