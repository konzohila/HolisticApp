using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;

namespace HolisticApp.ViewModels
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;

        private User CurrentUser { get; } 

        public AdminDashboardViewModel(User currentUser, IUserRepository userRepository, INavigation navigation)
        {
            CurrentUser = currentUser;
            _userRepository = userRepository;
            _navigation = navigation;
            Doctors = [];
        }

        [ObservableProperty]
        private ObservableCollection<User> _doctors;
        
        public string UserInitials => GetInitials(CurrentUser.Username);

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length == 0) return "";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }

        [RelayCommand]
        private async Task LoadDoctorsAsync()
        {
            var allUsers = await _userRepository.GetUsersAsync();
            var doctorList = allUsers.Where(u => u.Role == UserRole.Doctor).ToList();
            Doctors.Clear();
            foreach (var doctor in doctorList)
            {
                Doctors.Add(doctor);
            }
        }
        
        [RelayCommand]
        private async Task CreateDoctorAsync()
        {
            var services = (Application.Current as App)?.Handler?.MauiContext?.Services
                           ?? throw new InvalidOperationException("DI-Services nicht verfügbar.");
            
            var logger = services.GetService(typeof(ILogger<DoctorRegistrationPage>)) as ILogger<DoctorRegistrationPage>
                         ?? throw new InvalidOperationException("Logger für DoctorRegistrationPage nicht gefunden.");
            
            var doctorRegistrationPage = new DoctorRegistrationPage(logger);
            await _navigation.PushAsync(doctorRegistrationPage);
        }

        [RelayCommand]
        private async Task OpenUserMenuAsync()
        {
            await _navigation.PushAsync(new UserMenuPage(CurrentUser));
        }
    }
}
