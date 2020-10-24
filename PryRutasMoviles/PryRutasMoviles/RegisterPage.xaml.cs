using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                tblVehicle.IsVisible = true;
            }
            else
            {
                tblVehicle.IsVisible = false;
            }

        }
    }
}