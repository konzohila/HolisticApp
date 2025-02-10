using HolisticApp.Models;

namespace HolisticApp.Views
{
    public partial class PatientDetailPage
    {
        public PatientDetailPage(User patient)
        {
            InitializeComponent();
            BindingContext = patient;
        }
    }
}