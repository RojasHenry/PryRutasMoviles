using System.Threading.Tasks;
using Plugin.FacebookClient;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System;
using AiForms.Dialogs;
using PryRutasMoviles.Pages.Dialog;

namespace PryRutasMoviles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();

        public LoginPage()
        {
            InitializeComponent();

            string credentials = serviceLogin.GetCredentials();
            
            AutoLogin(credentials);
            
        }

        async void AutoLogin(string credentials)
        {
            if (!credentials.Equals(""))
            {
                var reusableLoading = Loading.Instance.Create<LoadingDialog>();

                reusableLoading.Show();

                User savedUser = JsonConvert.DeserializeObject<User>(credentials);

                var response = await serviceLogin
                        .LoginWithFirebaseCredentials(savedUser.Email, savedUser.Password);

                if (response)
                {
                    using (UserRepository userRepository = new UserRepository())
                    {
                        var user = await userRepository.GetUserByEmail(savedUser.Email);
                        if (user.UserType.Equals("Driver"))
                        {
                            reusableLoading.Hide();
                            var userType = await ShowModalType(Navigation);
                            serviceLogin.SaveCredentials(JsonConvert.SerializeObject(user));
                            await Navigation.PushAsync(new DriverTabbedPage(user, userType));
                        }
                        else
                        {
                            serviceLogin.SaveCredentials(JsonConvert.SerializeObject(user));
                            await Navigation.PushAsync(new PassengerTabbedPage(user));
                            reusableLoading.Hide();
                        }
                        
                    }
                }
            }
        }

        private void BtnRegister_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new RegistryPage(new User()));
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

        private void CleanEntries() {
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void EnableDisableActivityIndicator(bool flagActivityIndicator)
        {
            activity.IsEnabled = flagActivityIndicator;
            activity.IsRunning = flagActivityIndicator;
            activity.IsVisible = flagActivityIndicator;
        }

        private void EnableDisableControls(bool controlFlag) 
        {
            btnLogin.IsEnabled = controlFlag;
            btnRegister.IsEnabled = controlFlag;
            btnFBLogin.IsEnabled = controlFlag;
            BtnGoogleLogin.IsEnabled = controlFlag;
            txtEmail.IsEnabled = controlFlag;
            txtPassword.IsEnabled = controlFlag;
        }

        private async void BtnLogin_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && 
                    !string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    EnableDisableActivityIndicator(true);
                    EnableDisableControls(false);

                    var response = await serviceLogin
                        .LoginWithFirebaseCredentials(txtEmail.Text.Trim(), txtPassword.Text.Trim());

                    if (response)
                    {
                        using (UserRepository userRepository = new UserRepository())
                        {
                            var user = await userRepository.GetUserByEmail(txtEmail.Text.Trim());
                            EnableDisableActivityIndicator(false);
                            if (user.UserType.Equals("Driver"))
                            {
                                var userType = await ShowModalType(Navigation);
                                serviceLogin.SaveCredentials(JsonConvert.SerializeObject(user));
                                await Navigation.PushAsync(new DriverTabbedPage(user, userType));
                            }
                            else
                            {
                                serviceLogin.SaveCredentials(JsonConvert.SerializeObject(user));
                                await Navigation.PushAsync(new PassengerTabbedPage(user));
                            }
                            EnableDisableActivityIndicator(false);
                            EnableDisableControls(true);
                            CleanEntries();
                        }
                    }
                    else
                    {
                        EnableDisableActivityIndicator(false);
                        EnableDisableControls(true);
                        await DisplayAlert("Alert", "User or password incorrect", "Ok");
                    }
                }
                else
                {
                    EnableDisableActivityIndicator(false);
                    EnableDisableControls(true);
                    await DisplayAlert("Alert", "Please complete all entries", "Ok");
                }
            }
            catch
            {
                EnableDisableActivityIndicator(false);
                EnableDisableControls(true);
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        private async void BtnFBLogin_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                EnableDisableActivityIndicator(true);
                EnableDisableControls(false);

                FacebookResponse<bool> response =
                        await CrossFacebookClient.Current.LoginAsync(new string[] { "email", "public_profile" });

                if (response.Status == FacebookActionStatus.Completed)
                {
                    FacebookResponse<string> respInfo =
                        await CrossFacebookClient.Current.RequestUserDataAsync(
                                new string[] { "email", "first_name", "gender", "last_name", "birthday" },
                                new string[] { "email", "user_birthday" }
                            );

                    if (respInfo.Status == FacebookActionStatus.Completed)
                    {
                        var tokenAccess = CrossFacebookClient.Current.ActiveToken;
                        bool isLogged = await serviceLogin.LoginWithFirebaseFB(tokenAccess);

                        if (isLogged)
                        {
                            using (UserRepository userRepository = new UserRepository())
                            {
                                var userInBD = await userRepository.GetUserByEmail(serviceLogin.getCurrentUser());
                                if (userInBD != null)
                                {
                                    if (userInBD.UserType.Equals("Driver"))
                                    {
                                        EnableDisableActivityIndicator(false);
                                        EnableDisableControls(true);
                                        var whatUser = await ShowModalType(Navigation);
                                        await Navigation.PushAsync(new DriverTabbedPage(userInBD, whatUser));
                                    }
                                    else
                                    {
                                        EnableDisableActivityIndicator(false);
                                        EnableDisableControls(true);
                                        await Navigation.PushAsync(new PassengerTabbedPage(userInBD));
                                    }
                                    EnableDisableActivityIndicator(false);
                                    EnableDisableControls(true);
                                    CleanEntries();
                                }
                                else
                                {
                                    respInfo.Data = respInfo.Data
                                        .Replace("first_name", "FirstName")
                                        .Replace("last_name", "LastName");
                                    User user = JsonConvert.DeserializeObject<User>(respInfo.Data);
                                    user.IsFromSocialNetworks = true;
                                    EnableDisableActivityIndicator(false);
                                    EnableDisableControls(true);
                                    await Navigation.PushAsync(new RegistryPage(user));
                                }
                            }
                        }
                        else
                        {
                            EnableDisableActivityIndicator(false);
                            EnableDisableControls(true);
                            await DisplayAlert("Alerta", "Failed to login with Facebook", "Ok");
                        }
                    }
                    else
                    {
                        EnableDisableActivityIndicator(false);
                        EnableDisableControls(true);
                        await DisplayAlert("Alerta", respInfo.Message, "Ok");
                    }
                }
                else
                {
                    EnableDisableActivityIndicator(false);
                    EnableDisableControls(true);
                    await DisplayAlert("Alerta", response.Message, "Ok");
                }
            }
            catch (Exception ex)
            {
                EnableDisableActivityIndicator(false);
                EnableDisableControls(true);
                await DisplayAlert("Error", "An unexpected error has occurred" + ex.Message, "Ok");
            }
        }

        private async void BtnGoogleLogin_Clicked(object sender, System.EventArgs e)
        {
            EnableDisableActivityIndicator(true);
            EnableDisableControls(false);

            User user = await serviceLogin.LoginGoogle();

            try
            {
                if (user != null)
                {
                    using (UserRepository userRepository = new UserRepository())
                    {
                        var userRS = await userRepository.GetUserByEmail(user.Email);
                        if (userRS != null)
                        {
                            if (userRS.UserType.Equals("Driver"))
                            {
                                EnableDisableActivityIndicator(false);
                                EnableDisableControls(true);
                                var whatUser = await ShowModalType(Navigation);
                                await Navigation.PushAsync(new DriverTabbedPage(userRS, whatUser));

                            }
                            else
                            {
                                EnableDisableActivityIndicator(false);
                                EnableDisableControls(true);
                                await Navigation.PushAsync(new PassengerTabbedPage(userRS));
                            }

                            EnableDisableActivityIndicator(false);
                            EnableDisableControls(true);
                            CleanEntries();
                        }
                        else
                        {
                            EnableDisableActivityIndicator(false);
                            EnableDisableControls(true);
                            user.IsFromSocialNetworks = true;
                            await Navigation.PushAsync(new RegistryPage(user));
                        }

                    }
                }
                else
                {
                    EnableDisableActivityIndicator(false);
                    EnableDisableControls(true);
                    await DisplayAlert("Alerta", "Failed to login with Google", "Ok");
                }
                
            }
            catch (Exception ex)
            {
                EnableDisableActivityIndicator(false);
                EnableDisableControls(true);
                await DisplayAlert("Error", "An unexpected error has occurred"+ ex.Message, "Ok");
            }
        }

        
    }
}