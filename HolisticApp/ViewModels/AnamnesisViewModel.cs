using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace HolisticApp.ViewModels
{
    public partial class AnamnesisViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;
        public User CurrentUser { get; }

        [ObservableProperty]
        private string age;

        [ObservableProperty]
        private string selectedGender;

        [ObservableProperty]
        private string height;

        [ObservableProperty]
        private string weight;

        [ObservableProperty]
        private bool hasComplaint;

        [ObservableProperty]
        private string selectedComplaint;

        [ObservableProperty]
        private double severity = 1;

        public string[] GenderOptions { get; } = new string[] { "Männlich", "Weiblich", "Divers" };
        public string[] ComplaintOptions { get; } = new string[] { "Verdauungsbeschwerden", "Kopfschmerzen", "Rückenschmerzen" };

        public AnamnesisViewModel(User user, IUserRepository userRepository, INavigation navigation)
        {
            CurrentUser = user;
            _userRepository = userRepository;
            _navigation = navigation;

            // Vorbefüllung der Felder aus dem aktuellen User
            if (user.Age.HasValue)
                Age = user.Age.Value.ToString();
            SelectedGender = string.IsNullOrEmpty(user.Gender) ? GenderOptions[0] : user.Gender;
            Height = user.Height.HasValue ? user.Height.Value.ToString() : string.Empty;
            Weight = user.Weight.HasValue ? user.Weight.Value.ToString() : string.Empty;
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
        public async Task SaveAsync()
        {
            // Validierung und Update des Users
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
                    await App.Current.MainPage.DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
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
                await App.Current.MainPage.DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                Preferences.Set($"AnamnesisCompleted_{CurrentUser.Id}", true);
                await _navigation.PushAsync(new HomePage(CurrentUser));
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }
    }
}
