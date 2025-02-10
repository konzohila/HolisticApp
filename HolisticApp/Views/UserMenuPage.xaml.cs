using HolisticApp.Models;
using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class UserMenuPage
    {
        public UserMenuPage(User user)
        {
            InitializeComponent();
            BindingContext = new UserMenuViewModel(user, Navigation);
        }
    }
}