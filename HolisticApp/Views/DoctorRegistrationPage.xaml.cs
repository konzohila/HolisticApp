using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;  // Für Preferences
using System.Threading.Tasks;

namespace HolisticApp.Views
{
    public partial class DoctorRegistrationPage : ContentPage
    {
        public DoctorRegistrationPage()
        {
            InitializeComponent();
            // Den BindingContext setzen wir erst in OnAppearing, nachdem wir die Admin-Prüfung durchgeführt haben.
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Lese die aktuell eingeloggte User-ID aus den Preferences
            int loggedInUserId = Preferences.Get("LoggedInUserId", 0);
            if (loggedInUserId == 0)
            {
                await DisplayAlert("Fehler", "Kein Benutzer angemeldet.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // Greife über den DI-Container auf das IUserRepository zu
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;

            // Lade den aktuell eingeloggten Benutzer
            var currentUser = await userRepository.GetUserAsync(loggedInUserId);
            if (currentUser == null || currentUser.Role != UserRole.Admin)
            {
                await DisplayAlert("Unberechtigt", "Nur Administratoren können Doktoren registrieren.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // Nur wenn der Benutzer ein Admin ist, wird der BindingContext gesetzt.
            BindingContext = new DoctorRegistrationViewModel(userRepository, Navigation);
        }
    }
}