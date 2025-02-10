using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class HomePage
    {
        public HomePage(HomeViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}