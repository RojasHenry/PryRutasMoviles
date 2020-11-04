using System.Threading.Tasks;
using Plugin.FacebookClient;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new RegistryPage());
        }

        private async void BtnLogin_Clicked(object sender, System.EventArgs e)
        {
            EnableDisableActivityIndicator(true);
            EnableDisableControls(false);

            try
            {
                var respuesta = await serviceLogin.LoginWithFirebaseCredentials(txtEmail.Text.Trim(), txtPassword.Text.Trim());

                if (respuesta)
                {
                    using (UserRepository userRepository = new UserRepository())
                    {
                        var user = await userRepository.GetUserById(txtEmail.Text.Trim());
                        EnableDisableActivityIndicator(false);
                        if (user.UserType.Equals("Driver"))
                        {
                            var whatUser = await ShowModalType(Navigation);
                            await Navigation.PushAsync(new DriverTabbedPage(user, whatUser));
                           
                        }
                        else
                        {
                            await Navigation.PushAsync(new PassengerTabbedPage(user));
                        }
                    }
                }
                else
                {
                    EnableDisableActivityIndicator(false);
                    EnableDisableControls(true);
                    await DisplayAlert("Alert", "User or password incorrect", "Ok");
                    CleanEntries();
                }
                
            }
            catch 
            {
                EnableDisableActivityIndicator(false);
                EnableDisableControls(true);
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
                CleanEntries();
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
            txtEmail.IsEnabled = controlFlag;
            txtPassword.IsEnabled = controlFlag;
        }

        private async void btnPasajero_Clicked(object sender, System.EventArgs e)
        {
            
            User user = await serviceLogin.LoginGoogle();

            try
            {
                using (UserRepository userRepository = new UserRepository())
                {
                    var userRS = await userRepository.GetUserById(user.Email);
                    if(userRS != null)
                    {
                        if (user.UserType.Equals("Driver"))
                        {
                            var whatUser = await ShowModalType(Navigation);
                            await Navigation.PushAsync(new DriverTabbedPage(user, whatUser));

                        }
                        else
                        {
                            await Navigation.PushAsync(new PassengerTabbedPage(user));
                        }
                    }
                    else
                    {
                        await Navigation.PushAsync(new RegistryPage(user));
                    }
                    
                }
            }
            catch
            {

            }
        }

        private async void btnConductor_Clicked(object sender, System.EventArgs e)
        {
            FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(new string[] { "email", "public_profile" });

            if (response.Status == FacebookActionStatus.Completed && response.Message.Equals(""))
            {
                FacebookResponse<string> respInfo = await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "gender", "last_name", "birthday" }, new string[] { "email", "user_birthday" });
                if (respInfo.Status == FacebookActionStatus.Completed && respInfo.Message.Equals(""))
                {
                    var tokenaccess = CrossFacebookClient.Current.ActiveToken;

                    bool isloggued = await serviceLogin.LoginWithFirebaseFB(tokenaccess);

                    if (isloggued)
                    {
                        try
                        {
                            using (UserRepository userRepository = new UserRepository())
                            {
                                string email = serviceLogin.getCurrentUser();
                                var userRS = await userRepository.GetUserById(email);
                                if (userRS != null)
                                {
                                    if (userRS.UserType.Equals("Driver"))
                                    {
                                        var whatUser = await ShowModalType(Navigation);
                                        await Navigation.PushAsync(new DriverTabbedPage(userRS, whatUser));
                                    }
                                    else
                                    {
                                        await Navigation.PushAsync(new PassengerTabbedPage(userRS));
                                    }
                                }
                                else
                                {
                                    await Navigation.PushAsync(new RegistryPage(respInfo.Data));
                                }
                                
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Error al iniciar sesión en Facebook", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alerta", response.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Alerta",response.Message,"OK");
            }
        }

    }
}