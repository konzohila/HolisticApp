using HolisticApp.Models;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class HomePage
    {
        public HomePage(User user)
        {
            InitializeComponent();
            // Erstelle das ViewModel und setze die Initialen
            var viewModel = new HomeViewModel(user, Navigation)
            {
                UserInitials = GetInitials(user.Username)
            };
            BindingContext = viewModel;
        }

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length == 0) return "";
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return string.Concat(parts.Select(p => p[0])).ToUpper();
        }
    }
}