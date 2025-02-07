using HolisticApp.Models;
using Microsoft.Maui.Controls;

namespace HolisticApp.Views
{
    public partial class PatientDetailPage : ContentPage
    {
        public PatientDetailPage(User patient)
        {
            InitializeComponent();
            // Setze das übergebene Patient-Objekt als BindingContext
            BindingContext = patient;
        }
    }
}