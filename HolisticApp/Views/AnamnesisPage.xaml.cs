using HolisticApp.ViewModels;

namespace HolisticApp.Views
{
    public partial class AnamnesisPage
    {
        public AnamnesisPage(AnamnesisViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}