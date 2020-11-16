using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AiForms.Dialogs;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.Dialog;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class OffersTripPage : ContentPage
    {
        readonly ObservableCollection<Trip> tripOfferList = new ObservableCollection<Trip>();
        private readonly User _user;

        public OffersTripPage(User user)
        {
            InitializeComponent();
            OfferTrip.ItemsSource = tripOfferList;
            _user = user;
            GetTripsOffered();
            GetPassengerCurrentTrip();
        }

        private async void GetPassengerCurrentTrip()
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    Trip currentPassengerTrip = await tripRepository.GetPassengerCurrentTrip(_user);

                    if(currentPassengerTrip != null)
                    {
                        await Navigation.PushAsync(new TripAcceptedPage(currentPassengerTrip, _user));
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "An unexpected error has occurred" + e.Message, "Ok");
            }
        }

        private async void GetTripsOffered()
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    tripOfferList.Clear();
                    List<Trip> list = await tripRepository.GetTripsOffered();
                    list.ForEach(trip => tripOfferList.Add(trip));
                }
            }
            catch(Exception e)
            {
                await DisplayAlert("Error", "An unexpected error has occurred"+ e.Message, "Ok");
            }
        }

        public void OfferTrip_Refreshing(object sender, EventArgs e)
        {
            GetTripsOffered();
            OfferTrip.EndRefresh();
        }

        public async void OfferTrip_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem == null)
                    return;

                using (TripRepository tripRepository = new TripRepository())
                {

                    var selectedTrip = e.SelectedItem as Trip;
                    OfferTrip.SelectedItem = null;

                    var response = await tripRepository.TripIsEnable(selectedTrip.TripId);

                    if (!response)
                    {
                        await DisplayAlert("Alert", "The trip is no longer available", "Ok");
                        GetTripsOffered();
                        return;
                    }

                    int seatsAvailableOnATrip = await tripRepository.GetSeatsAvailableOnATrip(selectedTrip.TripId);

                    if (seatsAvailableOnATrip == 0)
                    {
                        await DisplayAlert("Alert", "Trip with no available seats", "Ok");
                        GetTripsOffered();
                        return;
                    }

                    var confirmTrip = await ConfirmPostTrip(Navigation, selectedTrip);

                    if (confirmTrip)
                    {
                        var reusableLoading = Loading.Instance.Create<LoadingDialog>();

                        reusableLoading.Show();
                        await tripRepository.AddPassengerOnATrip(_user, selectedTrip.TripId);
                        await Navigation.PushAsync(new TripAcceptedPage(selectedTrip, _user));
                        reusableLoading.Hide();
                    }                    
                }
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", "An unexpected error has occurred" + exc.Message, "Ok");
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
