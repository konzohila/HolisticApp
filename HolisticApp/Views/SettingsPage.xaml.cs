namespace HolisticApp.Views;

public partial class SettingsPage
{
    public SettingsPage(ViewModels.SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}