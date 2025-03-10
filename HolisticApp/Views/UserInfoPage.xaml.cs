using HolisticApp.ViewModels;

namespace HolisticApp.Views;

public partial class UserInfoPage
{
    public UserInfoPage(UserInfoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}