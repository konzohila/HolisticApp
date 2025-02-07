using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;

        public User CurrentUser { get; } // Der aktuell angemeldete Admin

        public AdminDashboardViewModel(User currentUser, IUserRepository userRepository, INavigation navigation)
        {
            CurrentUser = currentUser;
            _userRepository = userRepository;
            _navigation = navigation;
            Doctors = new ObservableCollection<User>();
        }

        [ObservableProperty]
        private ObservableCollection<User> doctors;

        // Property zur Anzeige der Initialen des Admins in der Toolbar
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
            var allUsers = await _userRepository.GetUsersAsync();
            var doctorList = allUsers.Where(u => u.Role == UserRole.Doctor).ToList();
            Doctors.Clear();
            foreach (var doctor in doctorList)
            {
                Doctors.Add(doctor);
            }
        }

        [RelayCommand]
        public async Task CreateDoctorAsync()
        {
            await _navigation.PushAsync(new DoctorRegistrationPage());
        }

        [RelayCommand]
        public async Task OpenUserMenuAsync()
        {
            await _navigation.PushAsync(new UserMenuPage(CurrentUser));
        }
    }
}
