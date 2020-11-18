using PryRutasMoviles.Extensions;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages
{
    public partial class RegistryPage : ContentPage
    {
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();
        private User _user;
        
        public RegistryPage(User user)
        {
            InitializeComponent();
            _user = user;
            if (_user.IsFromSocialNetworks)
            {
                txtLastName.Text = _user.LastName;
                txtFirstName.Text = _user.FirstName;
                btnNext.IsVisible=false;
                btnRegister.IsVisible = true;
                credentialsLayout.IsVisible = false;
            }
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

        private async void BtnRegister_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                EnableDisableActivityIndicator(true);
                btnRegister.IsEnabled = false;

                using (UserRepository userRepository = new UserRepository())
                {
                    if (!_user.IsFromSocialNetworks)
                    {
                        var userInBDD = await userRepository.GetUserByEmail(txtEmail.Text.Trim());

                        if (userInBDD != null)
                        {
                            await DisplayAlert("Alert", $"You are already registered  as {userInBDD.UserType}", "Ok");
                            EnableDisableActivityIndicator(false);
                            return;
                        }

                        var user = new User
                        {
                            Email = txtEmail.Text.Trim(),
                            FirstName = txtFirstName.Text.Trim(),
                            LastName = txtLastName.Text.Trim(),
                            Address = txtAddress.Text.Trim(),
                            Password = txtPassword.Text,
                            State = true,
                        };
                        if (driverSwitch.On)
                        {
                            user.UserType = "Driver";
                            user.Vehicle = new Vehicle
                            {
                                Registration = txtRegistration.Text.Trim(),
                                Brand = carBrand.Text.Trim(),
                                Color = carColor.Text.Trim(),
                                Year = carYear.Text.Trim(),
                                State = true
                            };
                        }
                        else
                            user.UserType = "Passenger";

                        var respuesta = await serviceLogin.CreateNewUserFirebase(user.Email, user.Password);

                        if (respuesta)
                        {
                            await userRepository.AddUser(user);
                            EnableDisableActivityIndicator(false);
                            await DisplayAlert("Alert", $"You are registered succesfully as {user.UserType}", "Ok");
                            CleanEntries();
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            EnableDisableActivityIndicator(false);
                            await DisplayAlert("Alert", "Error in the registry", "Ok");
                        }
                    }
                    else
                    {
                        if (IsValidForm()) 
                        {
                            _user.Address = txtAddress.Text.Trim();
                            _user.State = true;                            

                            if (driverSwitch.On)
                            {
                                _user.UserType = "Driver";
                                _user.Vehicle = new Vehicle
                                {
                                    Registration = txtRegistration.Text.Trim(),
                                    Brand = carBrand.Text.Trim(),
                                    Color = carColor.Text.Trim(),
                                    Year = carYear.Text.Trim(),
                                    State = true
                                };
                            }
                            else
                                _user.UserType = "Passenger";

                            await userRepository.AddUser(_user);
                            EnableDisableActivityIndicator(false);
                            await DisplayAlert("Alert", $"You are registered succesfully as {_user.UserType}", "Ok");
                            CleanEntries();
                            if (_user.UserType.Equals("Driver"))
                            {
                                var userType = await ShowModalType(Navigation);
                                await Navigation.PushAsync(new DriverTabbedPage(_user, userType));
                            }
                            else
                                await Navigation.PushAsync(new PassengerTabbedPage(_user));                                                        
                        }
                        else
                        {
                            EnableDisableActivityIndicator(false);
                            await DisplayAlert("Alert", "Please complete all entries", "Ok");
                        }                        
                    }
                }                
            }
            catch
            {
                EnableDisableActivityIndicator(false);
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
            finally
            {
                btnRegister.IsEnabled = true;
            }
        }

        private bool IsValidForm()
        {
           if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text)
                )
                return false;
            else if (driverSwitch.On)
                if (string.IsNullOrWhiteSpace(txtRegistration.Text) ||
                    (string.IsNullOrWhiteSpace(carYear.Text)    || carYear.Text.Equals("None"))  ||
                    (string.IsNullOrWhiteSpace(carBrand.Text)   || carBrand.Text.Equals("None")) ||
                    (string.IsNullOrWhiteSpace(carColor.Text)   || carColor.Text.Equals("None"))
                    )
                    return false;
            return true;
        }

        private void CleanEntries()
        {
            txtEmail.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtRegistration.Text = string.Empty;
            carYear.Text = "None";
            carBrand.Text = "None";
            carColor.Text = "None";
        }       

        private void EnableDisableActivityIndicator(bool flagActivityIndicator)
        {
            activity.IsEnabled = flagActivityIndicator;
            activity.IsRunning = flagActivityIndicator;
            activity.IsVisible = flagActivityIndicator;
        }

        private async void btnNext_Clicked(object sender, System.EventArgs e)
        {

            if (btnNext.Text.Equals("Next") && !_user.IsFromSocialNetworks)
            {
                if (IsValidForm())
                {
                    BasicsLayout.IsVisible = false;
                    vehicleLayout.IsVisible = false;
                    btnNext.Text = "Previous";
                    btnRegister.IsVisible = true;
                    credentialsLayout.IsVisible = true;
                    return;
                }
                else
                    await DisplayAlert("Alert", "Please complete all entries", "Ok");
            }
                       
            if (btnNext.Text.Equals("Previous") && !_user.IsFromSocialNetworks)
            {
                BasicsLayout.IsVisible = true;
                vehicleLayout.IsVisible = true;
                btnNext.IsVisible = true;
                btnNext.Text = "Next";
                btnRegister.IsVisible = false;
                credentialsLayout.IsVisible = false;
                return;
            }
        }

        private async Task<bool> ShowModalType(INavigation navigation)
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            void callback(bool didConfirm)
            {
                completionSource.TrySetResult(didConfirm);
            }

            var popup = new SelectTypeModal(callback);
            await navigation.PushPopupAsync(popup);
            return await completionSource.Task;
        }
    }
}
