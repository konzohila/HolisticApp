using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace HolisticApp.Views
{
    public partial class DoctorDashboardPage : ContentPage
    {
        public DoctorDashboardPage(User currentUser)
        {
            InitializeComponent();
            var app = Application.Current as App ?? throw new InvalidOperationException("Application is not available.");
            var services = app.Handler?.MauiContext?.Services ?? throw new InvalidOperationException("DI Services not available.");
            var userRepository = services.GetService(typeof(IUserRepository)) as IUserRepository 
                                 ?? throw new InvalidOperationException("UserRepository not found.");
            var invitationRepository = services.GetService(typeof(IInvitationRepository)) as IInvitationRepository 
                                       ?? throw new InvalidOperationException("InvitationRepository not found.");
            BindingContext = new DoctorDashboardViewModel(currentUser, userRepository, invitationRepository, Navigation);
        }

        private void OnPatientTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User selectedPatient)
            {
                if (BindingContext is DoctorDashboardViewModel viewModel)
                {
                    viewModel.OpenPatientDetailsCommand.Execute(selectedPatient);
                }
            }
            ((ListView)sender).SelectedItem = null;
        }
    }
}