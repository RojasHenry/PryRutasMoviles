using System;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages
{
    public partial class DriverTabbedPage : TabbedPage
    {
        private User _user;
        private bool _isDriver;

        ILoginSocialNetworks serviceLogin = DependencyService.Get<ILoginSocialNetworks>();

        public DriverTabbedPage(User user, bool isDriver)
        {
            InitializeComponent();
            _user = user;
            _isDriver = isDriver;

            if (_isDriver)
            {
                GetDriverCurrentTrip();
                GetPassengerCurrentTrip();
                NavigationPage registerRoute = new NavigationPage(new RegisterDriverRoutePage(_user));
                registerRoute.Title = "Current Trip";

                NavigationPage tripsDriver = new NavigationPage(new MyTripDriverPage(_user));
                tripsDriver.Title = "My Trips";

                Children.Add(registerRoute);
                Children.Add(tripsDriver);
            }
            else
            {
                GetDriverCurrentTrip();
                NavigationPage offerTrips = new NavigationPage(new OffersTripPage(_user));
                offerTrips.Title = "Trips offered";

                NavigationPage tripsPassenger = new NavigationPage(new MyTripPassengerPage(_user));
                tripsPassenger.Title = "My Trips";
                
                Children.Add(offerTrips);
                Children.Add(tripsPassenger);                
            }
        }

        private async void GetDriverCurrentTrip() 
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var currentDriverTrip = await tripRepository.GetDriverCurrentTrip(_user);
                
                if (currentDriverTrip != null)
                {
                    string message = (_isDriver) ? "You have one trip in progress": 
                                $"You have a trip in progress as Driver," +
                                $"\nPlease cancel or finish to continue as a Passenger";
                    
                    await DisplayAlert("Alert", message, "Ok");
                    await Navigation.PushAsync(new TripWaitingRoomPage(currentDriverTrip));
                }             
            }
        }

        private async void GetPassengerCurrentTrip() 
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var currentPassengerTrip = await tripRepository.GetPassengerCurrentTrip(_user);
                
                if (currentPassengerTrip != null)
                {
                    await DisplayAlert("Alert", $"You have a trip in progress as Passenger," +
                        $"\nPlease cancel or finish to continue as a Driver", "Ok");
                    await Navigation.PushAsync(new TripAcceptedPage(currentPassengerTrip, _user));                    
                }                
            }
        }
        
        void ToolbarItem_Clicked(object sender, EventArgs e)
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
