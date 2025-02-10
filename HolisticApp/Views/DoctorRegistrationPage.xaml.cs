using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class DoctorRegistrationPage
    {
        public DoctorRegistrationPage(DoctorRegistrationViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}