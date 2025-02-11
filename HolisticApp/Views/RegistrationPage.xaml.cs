using HolisticApp.ViewModels;

namespace HolisticApp.Views;

public partial class RegistrationPage
{
    public RegistrationPage(RegistrationViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}