using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class DoctorDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly INavigation _navigation;

        public User CurrentUser { get; } // Der aktuell angemeldete Doktor

        public DoctorDashboardViewModel(User currentUser,
                                        IUserRepository userRepository,
                                        IInvitationRepository invitationRepository,
                                        INavigation navigation)
        {
            CurrentUser = currentUser;
            _userRepository = userRepository;
            _invitationRepository = invitationRepository;
            _navigation = navigation;
            Patients = new ObservableCollection<User>();
            GeneratedInvitationLink = string.Empty; // Standardwert zugewiesen
        }

        [ObservableProperty]
        private ObservableCollection<User> patients;

        [ObservableProperty]
        private string generatedInvitationLink = string.Empty; // Initialisiert

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
        }

        [RelayCommand]
        public async Task LoadPatientsAsync()
        {
            var allUsers = await _userRepository.GetUsersAsync();
            var patientsList = allUsers
                .Where(u => u.Role == UserRole.Patient && u.MasterAccountId == CurrentUser.Id)
                .ToList();
            Patients.Clear();
            foreach (var patient in patientsList)
            {
                Patients.Add(patient);
            }
        }

        [RelayCommand]
        public async Task OpenPatientDetailsAsync(User patient)
        {
            if (patient != null)
            {
                await _navigation.PushAsync(new PatientDetailPage(patient));
            }
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            await _navigation.PushAsync(new UserMenuPage(CurrentUser));
        }
    }
}
