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
            // Wenn der Switch auf "Ja" steht, muss eine Option gewählt werden
            if (complaintSwitch.IsToggled)
            {
                if (complaintPicker.SelectedIndex == -1)
                {
                    await DisplayAlert("Fehler", "Bitte wählen Sie eine Beschwerde aus.", "OK");
                    return;
                }
                // Ausgewählte Beschwerde holen
                string selectedComplaint = complaintPicker.Items[complaintPicker.SelectedIndex];
                _currentUser.CurrentComplaint = selectedComplaint;
            }
            else
            {
                // Falls keine Beschwerden vorliegen
                _currentUser.CurrentComplaint = "Keine Beschwerden";
            }

            // Benutzer in der Datenbank aktualisieren (hierbei wird SaveUserAsync für ein Update aufgerufen)
            int result = await App.UserDatabase.SaveUserAsync(_currentUser);
            if (result > 0)
            {
                await DisplayAlert("Erfolg", "Ihre Informationen wurden gespeichert.", "OK");
                // Hier kannst du optional zur nächsten Seite navigieren
                // await Navigation.PushAsync(new NextPage());
            }
            else
            {
                await DisplayAlert("Fehler", "Beim Speichern ist ein Fehler aufgetreten.", "OK");
            }
        }
    }
}
