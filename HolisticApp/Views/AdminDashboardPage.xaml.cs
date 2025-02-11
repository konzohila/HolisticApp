using HolisticApp.ViewModels;

namespace HolisticApp.Views;

public partial class AdminDashboardPage
{
    public AdminDashboardPage(AdminDashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}