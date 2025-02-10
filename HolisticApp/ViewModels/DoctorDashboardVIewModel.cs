using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace HolisticApp.ViewModels
{
    public partial class DoctorDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly INavigation _navigation;
        private readonly ILogger<DoctorDashboardViewModel> _logger;
        [ObservableProperty]
        private ObservableCollection<User> _patients;
        [ObservableProperty]
        private string _generatedInvitationLink = string.Empty;
        public User CurrentUser { get; }

        public DoctorDashboardViewModel(User currentUser,
                                        IUserRepository userRepository,
                                        IInvitationRepository invitationRepository,
                                        INavigation navigation,
                                        ILogger<DoctorDashboardViewModel> logger)
        {
            CurrentUser = currentUser;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Patients = new ObservableCollection<User>();
            GeneratedInvitationLink = string.Empty;
        }

        public string UserInitials => GetInitials(CurrentUser.Username);

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length == 0)
                return string.Empty;
            if (parts.Length == 1)
                return parts[0].Substring(0, 1).ToUpper();
            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }

        [RelayCommand]
        public async Task GenerateInvitationAsync()
        {
            try
            {
                _logger.LogInformation("Doktor {DoctorId} erstellt Einladungstoken.", CurrentUser.Id);

                var token = Guid.NewGuid().ToString();
                var invitation = new Invitation
                {
                    Token = token,
                    MasterAccountId = CurrentUser.Id,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddDays(7),
                    IsUsed = false
                };

                await _invitationRepository.CreateInvitationAsync(invitation);
                GeneratedInvitationLink = $"https://yourapp.com/register?token={token}";

                _logger.LogInformation("Einladungstoken erfolgreich erstellt: {Link}", GeneratedInvitationLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in GenerateInvitationAsync für Doktor {DoctorId}", CurrentUser.Id);
            }
        }

        [RelayCommand]
        public async Task LoadPatientsAsync()
        {
            try
            {
                _logger.LogInformation("Doktor {DoctorId} lädt Patientenliste.", CurrentUser.Id);
                var allUsers = await _userRepository.GetUsersAsync();
                var patientsList = allUsers
                    .Where(u => u.Role == UserRole.Patient && u.MasterAccountId == CurrentUser.Id)
                    .ToList();

                Patients.Clear();
                foreach (var patient in patientsList)
                {
                    Patients.Add(patient);
                }
                _logger.LogInformation("LoadPatientsAsync erfolgreich: {Count} Patienten gefunden.", patientsList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in LoadPatientsAsync für Doktor {DoctorId}", CurrentUser.Id);
            }
        }

        [RelayCommand]
        public async Task OpenPatientDetailsAsync(User? patient)
        {
            try
            {
                if (patient != null)
                {
                    _logger.LogInformation("Doktor {DoctorId} öffnet PatientDetailPage für Patient {PatientId}",
                        CurrentUser.Id, patient.Id);
                    await _navigation.PushAsync(new PatientDetailPage(patient));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen der PatientDetailPage für Doktor {DoctorId}", CurrentUser.Id);
            }
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            try
            {
                _logger.LogInformation("Doktor {DoctorId} öffnet das User-Menü.", CurrentUser.Id);
                var services = Application.Current?.Handler?.MauiContext?.Services;
                if (services != null)
                {
                    var userMenuPage = services.GetRequiredService<UserMenuPage>();
                    await _navigation.PushAsync(userMenuPage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für Doktor {DoctorId}", CurrentUser.Id);
            }
        }
    }
}