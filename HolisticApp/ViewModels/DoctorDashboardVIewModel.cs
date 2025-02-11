using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using HolisticApp.Constants;
using HolisticApp.Services.Interfaces;

namespace HolisticApp.ViewModels;

public partial class DoctorDashboardViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly INavigationService _navigationService;
    private readonly ILogger<DoctorDashboardViewModel> _logger;
    [ObservableProperty]
    private ObservableCollection<User> _patients;
    [ObservableProperty]
    private string _generatedInvitationLink = string.Empty;
    private readonly IUserSession _userSession;

    public DoctorDashboardViewModel(IUserRepository userRepository,
        IInvitationRepository invitationRepository,
        INavigationService navigationService,
        ILogger<DoctorDashboardViewModel> logger, 
        IUserSession userSession)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userSession = userSession;

        Patients = [];
        GeneratedInvitationLink = string.Empty;
    }

    public string UserInitials => GetInitials(_userSession.CurrentUser?.Username);

    private string GetInitials(string? fullName)
    {
        var parts = fullName?.Split(' ');
        if (parts is { Length: 0 })
            return string.Empty;
        if (parts is { Length: 1 })
            return parts[0].Substring(0, 1).ToUpper();
        if (parts != null) return string.Concat(parts.Select(p => p[0])).ToUpper();
        return string.Empty;
    }

    [RelayCommand]
    private async Task GenerateInvitationAsync()
    {
        try
        {
            if (_userSession.CurrentUser != null)
            {
                _logger.LogInformation("Doktor {DoctorId} erstellt Einladungstoken.", _userSession.CurrentUser.Id);

                var token = Guid.NewGuid().ToString();
                var invitation = new Invitation
                {
                    Token = token,
                    MasterAccountId = _userSession.CurrentUser.Id,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddDays(7),
                    IsUsed = false
                };

                await _invitationRepository.CreateInvitationAsync(invitation);
                GeneratedInvitationLink = $"https://yourapp.com/register?token={token}";
            }

            _logger.LogInformation("Einladungstoken erfolgreich erstellt: {Link}", GeneratedInvitationLink);
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler in GenerateInvitationAsync für Doktor {DoctorId}",
                    _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    private async Task LoadPatientsAsync()
    {
        try
        {
            if (_userSession.CurrentUser != null)
            {
                _logger.LogInformation("Doktor {DoctorId} lädt Patientenliste.", _userSession.CurrentUser.Id);
                var allUsers = await _userRepository.GetUsersAsync();
                var patientsList = allUsers
                    .Where(u => u.Role == UserRole.Patient && u.MasterAccountId == _userSession.CurrentUser.Id)
                    .ToList();

                Patients.Clear();
                foreach (var patient in patientsList)
                {
                    Patients.Add(patient);
                }

                _logger.LogInformation("LoadPatientsAsync erfolgreich: {Count} Patienten gefunden.",
                    patientsList.Count);
            }
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler in LoadPatientsAsync für Doktor {DoctorId}", _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    private async Task OpenPatientDetailsAsync(User? patient)
    {
        try
        {
            if (patient != null)
            {
                if (_userSession.CurrentUser != null)
                    _logger.LogInformation("Doktor {DoctorId} öffnet PatientDetailPage für Patient {PatientId}",
                        _userSession.CurrentUser.Id, patient.Id);
                await _navigationService.NavigateToAsync(Routes.PatientDetailPage);
            }
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler beim Öffnen der PatientDetailPage für Doktor {DoctorId}",
                    _userSession.CurrentUser.Id);
        }
    }

    [RelayCommand]
    private async Task OpenUserMenuAsync()
    {
        try
        {
            if (_userSession.CurrentUser != null)
                _logger.LogInformation("Doktor {DoctorId} öffnet das User-Menü.", _userSession.CurrentUser.Id);
            await _navigationService.NavigateToAsync(Routes.UserMenuPage);
        }
        catch (Exception ex)
        {
            if (_userSession.CurrentUser != null)
                _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für Doktor {DoctorId}",
                    _userSession.CurrentUser.Id);
        }
    }
}