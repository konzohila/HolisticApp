using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class AnamnesisPage : ContentPage
    {
        private User _currentUser;

        // Übergib den aktuell angemeldeten User über den Konstruktor
        public AnamnesisPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        // Wird aufgerufen, wenn der Switch umgeschaltet wird
        private void OnComplaintToggled(object sender, ToggledEventArgs e)
        {
            bool hasComplaint = e.Value;
            complaintLabel.IsVisible = hasComplaint;
            complaintPicker.IsVisible = hasComplaint;
        }

        // Speichern-Button
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
                _currentUser.CurrentComplaint = selectedComplaint;
            }
            else
            {
                _currentUser.CurrentComplaint = "Keine Beschwerden";
            }

            int result = await App.UserDatabase.SaveUserAsync(_currentUser);
            if (result > 0)
            {
                // Setze Flag, dass die Anamnese abgeschlossen ist
                Preferences.Set("AnamnesisCompleted", true);

                await DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                // Navigiere zur HomePage
                await Navigation.PushAsync(new HomePage(_currentUser));
            }
            else
            {
                await DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }
    }
}
