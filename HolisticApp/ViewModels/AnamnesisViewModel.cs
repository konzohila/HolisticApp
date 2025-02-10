using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;

namespace HolisticApp.ViewModels
{
    public partial class AnamnesisViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;
        private User CurrentUser { get; }

        [ObservableProperty]
        private string _age = string.Empty; 

        [ObservableProperty]
        private string _selectedGender = string.Empty;

        [ObservableProperty]
        private string _height = string.Empty;

        [ObservableProperty]
        private string _weight = string.Empty;

        [ObservableProperty]
        private bool _hasComplaint;

        [ObservableProperty]
        private string _selectedComplaint = string.Empty; 

        [ObservableProperty]
        private double _severity = 1;

        public string[] GenderOptions { get; } = ["Männlich", "Weiblich", "Divers"];
        public string[] ComplaintOptions { get; } = ["Verdauungsbeschwerden", "Kopfschmerzen", "Rückenschmerzen"];

        public AnamnesisViewModel(User user, IUserRepository userRepository, INavigation navigation)
        {
            CurrentUser = user;
            _userRepository = userRepository;
            _navigation = navigation;

            if (user.Age.HasValue)
                Age = user.Age.Value.ToString();
            SelectedGender = string.IsNullOrEmpty(user.Gender) ? GenderOptions[0] : user.Gender;
            Height = user.Height.HasValue ? user.Height.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            Weight = user.Weight.HasValue ? user.Weight.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            if (!string.IsNullOrEmpty(user.CurrentComplaint) && user.CurrentComplaint != "Keine Beschwerden")
            {
                HasComplaint = true;
                var parts = user.CurrentComplaint.Split(" (Stärke: ");
                SelectedComplaint = parts[0];
                if (parts.Length > 1 && parts[1].EndsWith("/10)"))
                {
                    var severityStr = parts[1].Replace("/10)", "");
                    if (double.TryParse(severityStr, out double parsedSeverity))
                        Severity = parsedSeverity;
                }
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Verwende das aktuelle Fenster über Windows[0].Page statt Application.MainPage:
            var currentPage = Application.Current?.Windows[0].Page;
            if (int.TryParse(Age, out int parsedAge))
                CurrentUser.Age = parsedAge;
            else
                CurrentUser.Age = null;

            CurrentUser.Gender = SelectedGender;

            if (decimal.TryParse(Height, out decimal parsedHeight))
                CurrentUser.Height = parsedHeight;
            else
                CurrentUser.Height = null;

            if (decimal.TryParse(Weight, out decimal parsedWeight))
                CurrentUser.Weight = parsedWeight;
            else
                CurrentUser.Weight = null;

            if (HasComplaint)
            {
                if (string.IsNullOrEmpty(SelectedComplaint))
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
                    return;
                }
                CurrentUser.CurrentComplaint = $"{SelectedComplaint} (Stärke: {Severity}/10)";
            }
            else
            {
                CurrentUser.CurrentComplaint = "Keine Beschwerden";
            }

            int result = await _userRepository.SaveUserAsync(CurrentUser);
            if (result > 0)
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                Preferences.Set($"AnamnesisCompleted_{CurrentUser.Id}", true);
                await _navigation.PushAsync(new HomePage(CurrentUser));
            }
            else
            {
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }
    }
}
