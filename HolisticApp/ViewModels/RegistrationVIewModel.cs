using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Controls;
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
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string invitationToken = string.Empty;

        public RegistrationViewModel(IUserRepository userRepository, IInvitationRepository invitationRepository, INavigation navigation)
        {
            _userRepository = userRepository;
            _invitationRepository = invitationRepository;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            var currentPage = Application.Current?.Windows?[0]?.Page;
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }
            
            var user = new User()
            {
                Username = Username,
                Email = Email,
                PasswordHash = Password
            };

            if (!string.IsNullOrWhiteSpace(InvitationToken))
            {
                var invitation = await _invitationRepository.GetInvitationByTokenAsync(InvitationToken);
                if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.Now)
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Ungültiger oder abgelaufener Einladungstoken.", "OK");
                    return;
                }
                user.MasterAccountId = invitation.MasterAccountId;
                user.Role = UserRole.Patient;

                await _invitationRepository.MarkInvitationAsUsedAsync(invitation.Id);
            }
            else
            {
                user.Role = UserRole.Patient;
            }

            await _userRepository.SaveUserAsync(user);
            if (currentPage != null)
                await currentPage.DisplayAlert("Erfolg", "Benutzer erfolgreich registriert.", "OK");
            await _navigation.PopAsync();
        }
    }
}