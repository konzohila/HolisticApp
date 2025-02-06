using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class AnamnesisPage : ContentPage
    {
        private User _currentUser;
        private int _selectedSeverity = 1; // Standardwert

        public AnamnesisPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserData(); // Bestehende Daten in UI setzen
        }

        private void LoadUserData()
        {
            if (_currentUser.Age.HasValue)
                AgeEntry.Text = _currentUser.Age.ToString();
    
            if (!string.IsNullOrEmpty(_currentUser.Gender))
                GenderPicker.SelectedItem = _currentUser.Gender;

            if (_currentUser.Height.HasValue)
                HeightEntry.Text = _currentUser.Height.ToString();

            if (_currentUser.Weight.HasValue)
                WeightEntry.Text = _currentUser.Weight.ToString();

            if (!string.IsNullOrEmpty(_currentUser.CurrentComplaint) && _currentUser.CurrentComplaint != "Keine Beschwerden")
            {
                string[] complaintParts = _currentUser.CurrentComplaint.Split(" (Stärke: ");
                string savedComplaint = complaintParts[0];
                int savedSeverity = 1;

                if (complaintParts.Length > 1)
                {
                    string severityPart = complaintParts[1].Replace("/10)", "");
                    int.TryParse(severityPart, out savedSeverity);
                }

                if (complaintPicker.Items.Contains(savedComplaint))
                    complaintPicker.SelectedItem = savedComplaint;

                complaintSwitch.IsToggled = true;
                complaintLabel.IsVisible = true;
                complaintPicker.IsVisible = true;
                severityLabel.IsVisible = true;
                severitySlider.IsVisible = true;

                _selectedSeverity = savedSeverity;
                severitySlider.Value = savedSeverity;
                severityLabel.Text = $"Stärke der Beschwerden: {_selectedSeverity}";
            }
        }
        
        private void OnComplaintToggled(object sender, ToggledEventArgs e)
        {
            bool hasComplaint = e.Value;
            complaintLabel.IsVisible = hasComplaint;
            complaintPicker.IsVisible = hasComplaint;
            severityLabel.IsVisible = hasComplaint;
            severitySlider.IsVisible = hasComplaint;
        }

        private void OnSeverityChanged(object sender, ValueChangedEventArgs e)
        {
            _selectedSeverity = (int)e.NewValue;
            severityLabel.Text = $"Stärke der Beschwerden: {_selectedSeverity}";
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Alter speichern
            if (int.TryParse(AgeEntry.Text, out int age))
                _currentUser.Age = age;
            else
                _currentUser.Age = null;

            // Geschlecht speichern
            if (GenderPicker.SelectedIndex != -1)
                _currentUser.Gender = GenderPicker.Items[GenderPicker.SelectedIndex];

            // Größe speichern
            if (decimal.TryParse(HeightEntry.Text, out decimal height))
                _currentUser.Height = height;
            else
                _currentUser.Height = null;

            // Gewicht speichern
            if (decimal.TryParse(WeightEntry.Text, out decimal weight))
                _currentUser.Weight = weight;
            else
                _currentUser.Weight = null;

            // Beschwerden speichern
            if (complaintSwitch.IsToggled)
            {
                if (complaintPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
                    return;
                }
                string selectedComplaint = complaintPicker.Items[complaintPicker.SelectedIndex];
                _currentUser.CurrentComplaint = $"{selectedComplaint} (Stärke: {severitySlider.Value}/10)";
            }
            else
            {
                _currentUser.CurrentComplaint = "Keine Beschwerden";
            }

            // Speichere die Werte in der Datenbank
            int result = await App.UserDatabase.SaveUserAsync(_currentUser);
            if (result > 0)
            {
                await DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                Preferences.Set($"AnamnesisCompleted_{_currentUser.Id}", true);
                await Navigation.PushAsync(new HomePage(_currentUser));
            }
            else
            {
                await DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }

    }
}
