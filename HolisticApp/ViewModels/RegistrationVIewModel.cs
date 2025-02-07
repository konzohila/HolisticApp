using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using System;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly INavigation _navigation;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        // Neue Property für den Einladungstoken
        [ObservableProperty]
        private string invitationToken;

        public RegistrationViewModel(IUserRepository userRepository, IInvitationRepository invitationRepository, INavigation navigation)
        {
            _userRepository = userRepository;
            _invitationRepository = invitationRepository;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password // In Produktion bitte hashen!
            };

            // Falls ein Einladungstoken vorliegt, verarbeite diesen:
            if (!string.IsNullOrWhiteSpace(InvitationToken))
            {
                var invitation = await _invitationRepository.GetInvitationByTokenAsync(InvitationToken);
                if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.Now)
                {
                    await App.Current.MainPage.DisplayAlert("Fehler", "Ungültiger oder abgelaufener Einladungstoken.", "OK");
                    return;
                }
                // Setze den MasterAccountId des neuen Users auf den aus der Einladung
                user.MasterAccountId = invitation.MasterAccountId;
                // Setze die Rolle des Benutzers auf Patient
                user.Role = UserRole.Patient;

                // Markiere die Einladung als verwendet
                await _invitationRepository.MarkInvitationAsUsedAsync(invitation.Id);
            }
            else
            {
                // Optional: Falls ausschließlich registriert werden soll, wenn ein Token vorliegt,
                // könnte man hier die Registrierung verweigern.
                user.Role = UserRole.Patient;
            }

            await _userRepository.SaveUserAsync(user);
            await App.Current.MainPage.DisplayAlert("Erfolg", "Benutzer erfolgreich registriert.", "OK");
            await _navigation.PopAsync();
        }
    }
}
