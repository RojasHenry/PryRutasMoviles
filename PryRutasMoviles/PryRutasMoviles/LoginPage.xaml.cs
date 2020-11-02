using PryRutasMoviles.Repositories;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new RegisterPage());
        }

        private async void BtnLogin_Clicked(object sender, System.EventArgs e)
        {
            EnableDisableActivityIndicator(true);
            EnableDisableControls(false);

            try
            {
                using (UserRepository userRepository = new UserRepository())
                {
                    var user = await userRepository.GetUserById(txtEmail.Text);
                    if (user != null && user.Password.Equals(txtPassword.Text.Trim()))
                    {
                        if (user.UserType.Equals("Driver"))
                            EnableDisableActivityIndicator(false);
                        await Navigation.PushAsync(new RegisterRoutePage(user));
                    }
                    else 
                    {
                        EnableDisableActivityIndicator(false);
                        EnableDisableControls(true);
                        await DisplayAlert("Alert", "User or password incorrect", "Ok");
                        CleanEntries();
                    }                    
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

        private void btnPasajero_Clicked(object sender, System.EventArgs e)
        {

        }

        private void btnConductor_Clicked(object sender, System.EventArgs e)
        {

        }
    }
}