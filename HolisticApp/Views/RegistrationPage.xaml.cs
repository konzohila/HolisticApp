using HolisticApp.Data.Interfaces;
using HolisticApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Views
{
    public partial class RegistrationPage
    {
        // Optional: Der Konstruktor kann einen Einladungstoken entgegennehmen
        public RegistrationPage(string? invitationToken = null)
        {
            InitializeComponent();
            var app = Application.Current as App 
                      ?? throw new InvalidOperationException("Application ist nicht vom erwarteten Typ.");
            var services = app.Handler?.MauiContext?.Services 
                           ?? throw new InvalidOperationException("DI-Services nicht verfügbar.");
            var userRepository = services.GetService(typeof(IUserRepository)) as IUserRepository 
                                 ?? throw new InvalidOperationException("UserRepository nicht gefunden.");
            var invitationRepository = services.GetService(typeof(IInvitationRepository)) as IInvitationRepository 
                                       ?? throw new InvalidOperationException("InvitationRepository nicht gefunden.");
            var logger = services.GetService(typeof(ILogger<RegistrationViewModel>)) as ILogger<RegistrationViewModel>
                         ?? throw new InvalidOperationException("Logger für RegistrationViewModel nicht gefunden.");
    
            var viewModel = new RegistrationViewModel(userRepository, invitationRepository, Navigation, logger);
    
            if (!string.IsNullOrWhiteSpace(invitationToken))
            {
                viewModel.InvitationToken = invitationToken;
            }
            BindingContext = viewModel;
        }
    }
}