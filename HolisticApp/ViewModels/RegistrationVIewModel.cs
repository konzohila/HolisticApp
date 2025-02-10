using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly INavigation _navigation;
        private readonly ILogger<RegistrationViewModel> _logger;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string invitationToken = string.Empty;

        public RegistrationViewModel(IUserRepository userRepository,
                                     IInvitationRepository invitationRepository,
                                     INavigation navigation,
                                     ILogger<RegistrationViewModel> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            var currentPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
            try
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password))
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                    _logger.LogWarning("Registrierung fehlgeschlagen: Mindestens ein Feld ist leer. (Username: {Username}, Email: {Email})", Username, Email);
                    return;
                }
                
                _logger.LogInformation("Registrierungsversuch für Email: {Email} gestartet.", Email);
                var user = new User()
                {
                    Username = Username,
                    Email = Email,
                    PasswordHash = Password
                };

                if (!string.IsNullOrWhiteSpace(InvitationToken))
                {
                    _logger.LogDebug("Registrierung mit Einladungstoken: {InvitationToken}", InvitationToken);
                    var invitation = await _invitationRepository.GetInvitationByTokenAsync(InvitationToken);
                    if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.Now)
                    {
                        if (currentPage != null)
                            await currentPage.DisplayAlert("Fehler", "Ungültiger oder abgelaufener Einladungstoken.", "OK");
                        _logger.LogWarning("Registrierung fehlgeschlagen: Ungültiger oder abgelaufener Token: {InvitationToken}", InvitationToken);
                        return;
                    }
                    user.MasterAccountId = invitation.MasterAccountId;
                    user.Role = UserRole.Patient;
                    await _invitationRepository.MarkInvitationAsUsedAsync(invitation.Id);
                    _logger.LogInformation("Einladungstoken {InvitationToken} wurde erfolgreich als verwendet markiert.", InvitationToken);
                }
                else
                {
                    user.Role = UserRole.Patient;
                }

                int result = await _userRepository.SaveUserAsync(user);
                if (result > 0)
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Erfolg", "Benutzer erfolgreich registriert.", "OK");
                    _logger.LogInformation("Registrierung für Email: {Email} erfolgreich abgeschlossen.", Email);
                    await _navigation.PopAsync();
                }
                else
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Fehler beim Registrieren des Benutzers.", "OK");
                    _logger.LogError("Registrierung für Email: {Email} schlug fehl. Keine Zeilen wurden in die DB geschrieben.", Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception während der Registrierung für Email: {Email}", Email);
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
            }
        }
    }
}