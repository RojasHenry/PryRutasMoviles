using System;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriverTabbedPage : TabbedPage
    {
        private User _user;

        public DriverTabbedPage(User user, bool isDriver)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Title = $"Welcome, {user.FullName}";
            _user = user;
            RedirectUserWithBothRoles(_user, isDriver);
        }

        private async void RedirectUserWithBothRoles(User user, bool isDriver)
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var currentDriverTrip = await tripRepository.GetDriverCurrentTrip(user);
                var currentPassengerTrip = await tripRepository.GetPassengerCurrentTrip(user);

                if (isDriver)
                {
                    if (currentDriverTrip != null)
                    {
                        await Navigation.PushAsync(new TripWaitingRoomPage(currentDriverTrip));
                        return;
                    }

                    if (currentPassengerTrip != null)
                    {
                        await DisplayAlert("Alert", $"You have a trip in progress as Passenger," +
                            $"\nPlease cancel or finish to continue as a Driver", "Ok");
                        await Navigation.PushAsync(new TripAcceptedPage(currentPassengerTrip, user));
                        return;
                    }

                    NavigationPage registerRoute = new NavigationPage(new RegisterDriverRoutePage(user));
                    registerRoute.Title = "Current Trip";

                    NavigationPage tripsDriver = new NavigationPage(new MyTripDriverPage());
                    tripsDriver.Title = "My Trips";

                    Children.Add(registerRoute);
                    Children.Add(tripsDriver);
                }
                else
                {
                    if (currentDriverTrip != null) 
                    {
                        await DisplayAlert("Alert", $"You have a trip in progress as Driver," +
                            $"\nPlease cancel or finish to continue as a Passenger", "Ok");
                        await Navigation.PushAsync(new TripWaitingRoomPage(currentDriverTrip));
                        return;
                    }

                    await Navigation.PushAsync(new PassengerTabbedPage(user));                                        
                }
            }
        }

        void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfileUserPage(_user));
        }
    }
}
