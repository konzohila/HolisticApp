using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using HolisticApp.Views;
using Microsoft.Extensions.Logging;

namespace HolisticApp.ViewModels
{
    public partial class AnamnesisViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly INavigation _navigation;
        private readonly ILogger<AnamnesisViewModel> _logger;

        public User CurrentUser { get; }

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

        public string[] GenderOptions { get; } = { "Männlich", "Weiblich", "Divers" };
        public string[] ComplaintOptions { get; } = { "Verdauungsbeschwerden", "Kopfschmerzen", "Rückenschmerzen" };

        public AnamnesisViewModel(User user,
                                  IUserRepository userRepository,
                                  INavigation navigation,
                                  ILogger<AnamnesisViewModel> logger,
                                  HomePage homepage)
        {
            CurrentUser = user;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Fülle die Felder mit aktuellen User-Daten
            if (user.Age.HasValue) _age = user.Age.Value.ToString();
            SelectedGender = string.IsNullOrEmpty(user.Gender) ? GenderOptions[0] : user.Gender;
            Height = user.Height?.ToString() ?? string.Empty;
            Weight = user.Weight?.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(user.CurrentComplaint) && user.CurrentComplaint != "Keine Beschwerden")
            {
                _hasComplaint = true;
                var parts = user.CurrentComplaint.Split(" (Stärke: ");
                SelectedComplaint = parts[0];
                if (parts.Length > 1 && parts[1].EndsWith("/10)"))
                {
                    var severityStr = parts[1].Replace("/10)", "");
                    if (double.TryParse(severityStr, out double parsedSeverity))
                        _severity = parsedSeverity;
                }
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            var currentPage = Application.Current?.Windows?[0]?.Page;
            try
            {
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
                        _logger.LogWarning("Anamnese konnte nicht gespeichert werden (keine Beschwerde ausgewählt).");
                        return;
                    }
                    CurrentUser.CurrentComplaint = $"{SelectedComplaint} (Stärke: {Severity}/10)";
                }
                else
                {
                    CurrentUser.CurrentComplaint = "Keine Beschwerden";
                }

                _logger.LogInformation("Speichere Anamnese-Infos für User {UserId}", CurrentUser.Id);
                int result = await _userRepository.SaveUserAsync(CurrentUser);
                if (result > 0)
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                    Preferences.Set($"AnamnesisCompleted_{CurrentUser.Id}", true);

                    _logger.LogInformation("Anamnese erfolgreich gespeichert für User {UserId}. Navigiere HomePage.", CurrentUser.Id);
                    await _navigation.PushAsync(homePage);
                }
                else
                {
                    if (currentPage != null)
                        await currentPage.DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
                    _logger.LogError("Anamnese: Fehler beim Speichern in DB für User {UserId}.", CurrentUser.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unerwarteter Fehler beim Speichern der Anamnese für User {UserId}.", CurrentUser.Id);
                if (currentPage != null)
                    await currentPage.DisplayAlert("Fehler", "Ein unerwarteter Fehler ist aufgetreten.", "OK");
            }
        }
    }
}