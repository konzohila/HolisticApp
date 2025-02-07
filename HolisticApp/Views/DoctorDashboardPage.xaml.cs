using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class DoctorDashboardPage : ContentPage
    {
        public DoctorDashboardPage(User currentUser)
        {
            InitializeComponent();
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            var invitationRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IInvitationRepository)) as IInvitationRepository;
            BindingContext = new DoctorDashboardViewModel(currentUser, userRepository, invitationRepository, Navigation);
        }

        private void OnPatientTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User selectedPatient)
            {
                // Rufe den Command im ViewModel auf
                if (BindingContext is DoctorDashboardViewModel viewModel)
                {
                    viewModel.OpenPatientDetailsCommand.Execute(selectedPatient);
                }
            }
            // Deselect the tapped item
            ((ListView)sender).SelectedItem = null;
        }
    }
}