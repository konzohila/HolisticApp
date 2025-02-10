using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class UserMenuPage
    {
        public UserMenuPage(UserMenuViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}