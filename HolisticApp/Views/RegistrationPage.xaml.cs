using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class RegistrationPage : ContentPage
    {
        // Optional: Der Konstruktor kann einen Einladungstoken entgegennehmen
        public RegistrationPage(string invitationToken = null)
        {
            InitializeComponent();
            var userRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            var invitationRepository = (Application.Current as App)
                .Handler.MauiContext.Services.GetService(typeof(IInvitationRepository)) as IInvitationRepository;
            var viewModel = new RegistrationViewModel(userRepository, invitationRepository, Navigation);
            if (!string.IsNullOrWhiteSpace(invitationToken))
            {
                viewModel.InvitationToken = invitationToken;
            }
            BindingContext = viewModel;
        }
    }
}