using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class DoctorDashboardPage
    {
        public DoctorDashboardPage(DoctorDashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}