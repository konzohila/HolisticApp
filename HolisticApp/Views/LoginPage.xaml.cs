using HolisticApp.ViewModels;

namespace HolisticApp.Views;

public partial class LoginPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}