using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class AnamnesisPage : ContentPage
    {
        private User _currentUser;
        private int _selectedSeverity = 1; // Standardwert für die Stärke der Beschwerden

        public AnamnesisPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
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
            if (complaintSwitch.IsToggled)
            {
                if (complaintPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
                    return;
                }
                string selectedComplaint = complaintPicker.Items[complaintPicker.SelectedIndex];
                _currentUser.CurrentComplaint = $"{selectedComplaint} (Stärke: {_selectedSeverity}/10)";
            }
            else
            {
                _currentUser.CurrentComplaint = "Keine Beschwerden";
            }

            int result = await App.UserDatabase.SaveUserAsync(_currentUser);
            if (result > 0)
            {
                await DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                await Navigation.PushAsync(new HomePage(_currentUser));
            }
            else
            {
                await DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }
    }
}
