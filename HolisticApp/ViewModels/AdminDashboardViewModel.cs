using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;



namespace HolisticApp.ViewModels
{
    public abstract partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;
        private readonly ILogger<AdminDashboardViewModel> _logger;
        private readonly DoctorRegistrationPage _doctorRegistrationPage;
        private readonly UserMenuPage _userMenuPage;
        [ObservableProperty]
        private ObservableCollection<User> _doctors;
        public User CurrentUser { get; }

        public AdminDashboardViewModel(User currentUser,
                                       IUserRepository userRepository,
                                       INavigation navigation,
                                       ILogger<AdminDashboardViewModel> logger, DoctorRegistrationPage doctorRegistrationPage, UserMenuPage userMenuPage)
        {
            CurrentUser = currentUser;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _doctorRegistrationPage = doctorRegistrationPage;
            _userMenuPage = userMenuPage;

            Doctors = new ObservableCollection<User>();
        }

        public string UserInitials => GetInitials(CurrentUser.Username);

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length == 0) return "";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }

        [RelayCommand]
        public async Task LoadDoctorsAsync()
        {
            try
            {
                _logger.LogInformation("Lade alle Benutzer und filtere Doktoren für Admin {AdminId}", CurrentUser.Id);
                var allUsers = await _userRepository.GetUsersAsync();
                var doctorList = allUsers.Where(u => u.Role == UserRole.Doctor).ToList();

                Doctors.Clear();
                foreach (var doctor in doctorList)
                {
                    Doctors.Add(doctor);
                }
                _logger.LogInformation("LoadDoctorsAsync erfolgreich: {Count} Doktoren gefunden.", doctorList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler in LoadDoctorsAsync für Admin {AdminId}", CurrentUser.Id);
            }
        }

        [RelayCommand]
        public async Task CreateDoctorAsync()
        {
            try
            {
                _logger.LogInformation("Admin {AdminId} navigiert zur DoctorRegistrationPage.", CurrentUser.Id);
                await _navigation.PushAsync(_doctorRegistrationPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Navigieren zu DoctorRegistrationPage.");
            }
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            try
            {
                _logger.LogInformation("Admin {AdminId} öffnet das User-Menü.", CurrentUser.Id);
                await _navigation.PushAsync(_userMenuPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen des User-Menüs für Admin {AdminId}", CurrentUser.Id);
            }
        }
    }
}