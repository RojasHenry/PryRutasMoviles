using PryRutasMoviles.Extensions;
using PryRutasMoviles.Models;
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
            tblVehicle.IsVisible = e.Value;
        }

        private async void ViewCellCarYear_Tapped(object sender, System.EventArgs e)
        {
            var page = new CarYearList();
            
            page.CarYears.ItemSelected += (source, args) =>
            {
                carYear.Text = args.SelectedItem.ToString();
                Navigation.PopAsync();
            };
            
            await Navigation.PushAsync(page);
        }

        private async void ViewCellCarBrand_Tapped(object sender, System.EventArgs e)
        {
            var page = new CarBrandList();

            page.CarBrands.ItemSelected += (source, args) =>
            {
                carBrand.Text = args.SelectedItem.ToString();
                Navigation.PopAsync();
            };

            await Navigation.PushAsync(page);
        }

        private async void ViewCellCarColor_Tapped(object sender, System.EventArgs e)
        {
            var page = new CarColorList();

            page.CarColors.ItemSelected += (source, args) =>
            {
                carColor.Text = args.SelectedItem.ToString();
                Navigation.PopAsync();
            };

            await Navigation.PushAsync(page);
        }

        private void BtnRegister_Clicked(object sender, System.EventArgs e)
        {
            if (AreEntriesComplete())
            {
                if (driverSwitch.On)
                {
                    User driver = new User
                    {
                        DriverId = txtId.Text,
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Address = txtAddress.Text,
                        UserName = txtUser.Text,
                        Password = txtPassword.Text,
                        State = true,
                        Vehicle = new Vehicle {
                            Registration = txtRegistration.Text,
                            Brand = carBrand.Text,
                            Color = carColor.Text,
                            Year = carYear.Text,
                            State = true
                        }
                    };
                }                
            }
            else
            {
                DisplayAlert("Alert", "Please complete all entries", "Ok");
            }
        }

        private bool AreEntriesComplete() 
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtUser.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text)
                )
                return false;
            else if (driverSwitch.On)
                if ((string.IsNullOrWhiteSpace(carYear.Text) || carYear.Text.Equals("None")) ||
                    (string.IsNullOrWhiteSpace(carBrand.Text) || carBrand.Text.Equals("None")) ||
                    (string.IsNullOrWhiteSpace(carColor.Text) || carColor.Text.Equals("None")))
                    return false;
            return true;                
        }
    }
}