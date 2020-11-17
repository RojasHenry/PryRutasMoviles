using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using System;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages
{
    public partial class PassengerTabbedPage : TabbedPage
    {
        private User _user;
        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();
        public PassengerTabbedPage(User user)
        {
            InitializeComponent();
            _user = user;            

            NavigationPage offerTrips = new NavigationPage(new OffersTripPage(_user));
            offerTrips.Title = "Trips offered";

            NavigationPage tripsPassenger = new NavigationPage(new MyTripPassengerPage(_user));
            tripsPassenger.Title = "My Trips";

            Children.Add(offerTrips);
            Children.Add(tripsPassenger);            
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfileUserPage(_user));
        }

        async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            serviceLogin.Logout();
            serviceLogin.DeleteCredentials();
            await Navigation.PopToRootAsync();
        }
    }
}
