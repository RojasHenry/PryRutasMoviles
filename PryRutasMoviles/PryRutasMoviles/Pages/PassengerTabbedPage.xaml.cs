using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassengerTabbedPage : TabbedPage
    {
        User _user;
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();

        public PassengerTabbedPage(User user)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            Title = $"Welcome, {user.FullName}"; 

            NavigationPage offerTrips = new NavigationPage(new OffersTripPage(user));
            offerTrips.Title = "Trips offered";

            NavigationPage tripsPassenger = new NavigationPage(new MyTripPassengerPage());
            tripsPassenger.Title = "My Trips";

            Children.Add(offerTrips);
            Children.Add(tripsPassenger);
            _user = user;
        }

        private void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProfileUserPage(_user));
        }

        async void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            serviceLogin.Logout();
            serviceLogin.DeleteCredentials();
            await Navigation.PopToRootAsync();
        }
    }
}
