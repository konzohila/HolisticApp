using HolisticApp.Models;

namespace HolisticApp.Views
{
    public partial class PatientDetailPage
    {
        public PatientDetailPage(User patient)
        {
            InitializeComponent();
            // Setze das übergebene Patient-Objekt als BindingContext
            BindingContext = patient;
        }
    }
}