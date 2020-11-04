using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class OffersTripPage : ContentPage
    {
        readonly ObservableCollection<Trip> tripOfferList = new ObservableCollection<Trip>();
        User userActual;
        public OffersTripPage(User user)
        {
            InitializeComponent();

            OfferTrip.ItemsSource = tripOfferList;
            userActual = user;
            GetTripsOfferted();

            ActualUserHasTrip();
        }

        async private void ActualUserHasTrip()
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    Trip actualTrip = await tripRepository.PassengerHasATrip(userActual);

                    if(actualTrip != null)
                    {
                        await Navigation.PushAsync(new TripAcceptedPage(actualTrip, userActual));
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "An unexpected error has occurred" + e.Message, "Ok");
            }
        }

        private async void GetTripsOfferted()
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    tripOfferList.Clear();
                    List<Trip> list = await tripRepository.GetTripsByState("Posted");
                    list.ForEach(trip => tripOfferList.Add(trip));
                }
            }
            catch(Exception e)
            {
                await DisplayAlert("Error", "An unexpected error has occurred"+ e.Message, "Ok");
            }
        }

        void OfferTrip_Refreshing(System.Object sender, System.EventArgs e)
        {
            GetTripsOfferted();
        }

        async void OfferTrip_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var item = (Trip)e.SelectedItem;

            var confirmTrip = await ConfirmPostTrip(Navigation, item);

            if (confirmTrip)
            {
                try
                {
                    using (TripRepository tripRepository = new TripRepository())
                    {
                        int numSeats = await tripRepository.SeatsOcuppateds(item.TripId);

                        if(numSeats == item.SeatsAvailables)
                        {
                            await DisplayAlert("Alert", "Trip with no available seats", "Ok");
                        }
                        else
                        {
                            bool response = await tripRepository.AddPassenger(userActual,item.TripId);
                            if (response)
                            {
                                await Navigation.PushAsync(new TripAcceptedPage(item,userActual));
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Trip with no available seats", "Ok");
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    await DisplayAlert("Error", "An unexpected error has occurred" + exc.Message, "Ok");
                }
            }
        }

        private async Task<bool> ConfirmPostTrip(INavigation navigation, Trip trip)
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            void callback(bool didConfirm)
            {
                completionSource.TrySetResult(didConfirm);
            }

            var popup = new DetailRouteModal(callback, trip);
            await navigation.PushPopupAsync(popup);
            return await completionSource.Task;
        }
    }
}
