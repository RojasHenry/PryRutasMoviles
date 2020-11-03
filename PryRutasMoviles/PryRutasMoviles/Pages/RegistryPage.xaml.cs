using System;
using System.Collections.Generic;
using PryRutasMoviles.Extensions;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages
{
    public partial class RegistryPage : ContentPage
    {
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();

        bool isFromSocialNet = false;

        public RegistryPage()
        {
            InitializeComponent();
        }

        public RegistryPage(User userGoogle)
        {
            InitializeComponent();

            txtEmail.Text = userGoogle.Email;
            txtLastName.Text = userGoogle.LastName;
            txtFirstName.Text = userGoogle.FirstName;

            txtEmail.IsEnabled = false;
            txtLastName.IsEnabled = false;
            txtFirstName.IsEnabled = false;
            tblCredentials.IsVisible = false;
        }

        public RegistryPage(string userFb)
        {
            InitializeComponent();

            UserFB userFB = JsonConvert.DeserializeObject<UserFB>(userFb);

            txtEmail.Text = userFB.email;
            txtLastName.Text = userFB.last_name;
            txtFirstName.Text = userFB.first_name;

            txtEmail.IsEnabled = false;
            txtLastName.IsEnabled = false;
            txtFirstName.IsEnabled = false;
            tblCredentials.IsVisible = false;
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
            EnableDisableActivityIndicator(true);
            btnRegister.IsEnabled = false;

            try
            {
                using (UserRepository userRepository = new UserRepository())
                {
                    if (IsValidForm())
                    {
                        var userInBDD = await userRepository.GetUserById(txtEmail.Text.ToUpper().Trim());

                        if (userInBDD != null)
                        {
                            await DisplayAlert("Alert", $"You are already registered  as {userInBDD.UserType}", "Ok");
                            return;
                        }

                        var user = new User
                        {
                            Email = txtEmail.Text,
                            FirstName = txtFirstName.Text,
                            LastName = txtLastName.Text,
                            Address = txtAddress.Text,
                            Password = txtPassword.Text,
                            State = true,
                        };
                        if (driverSwitch.On)
                        {
                            user.UserType = "Driver";
                            user.Vehicle = new Vehicle
                            {
                                Registration = txtRegistration.Text,
                                Brand = carBrand.Text,
                                Color = carColor.Text,
                                Year = carYear.Text,
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
                        EnableDisableActivityIndicator(false);
                        await DisplayAlert("Alert", "Please complete all entries", "Ok");
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
            if (!isFromSocialNet)
            {
               if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
               string.IsNullOrWhiteSpace(txtFirstName.Text) ||
               string.IsNullOrWhiteSpace(txtLastName.Text) ||
               string.IsNullOrWhiteSpace(txtAddress.Text) ||
               string.IsNullOrWhiteSpace(txtPassword.Text)
               )
                    return false;
                else if (driverSwitch.On)
                    if ((string.IsNullOrWhiteSpace(carYear.Text) || carYear.Text.Equals("None")) ||
                        (string.IsNullOrWhiteSpace(carBrand.Text) || carBrand.Text.Equals("None")) ||
                        (string.IsNullOrWhiteSpace(carColor.Text) || carColor.Text.Equals("None")))
                        return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
               string.IsNullOrWhiteSpace(txtFirstName.Text) ||
               string.IsNullOrWhiteSpace(txtLastName.Text) ||
               string.IsNullOrWhiteSpace(txtAddress.Text)
               )
                    return false;
                else if (driverSwitch.On)
                    if ((string.IsNullOrWhiteSpace(carYear.Text) || carYear.Text.Equals("None")) ||
                        (string.IsNullOrWhiteSpace(carBrand.Text) || carBrand.Text.Equals("None")) ||
                        (string.IsNullOrWhiteSpace(carColor.Text) || carColor.Text.Equals("None")))
                        return false;
            }
           
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
    }
}
