using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private static Stopwatch stopWatch = new Stopwatch();
        private const int defaultTimespan = 10;
        private ObservableCollection<Trip> _tripOfferList;
        private readonly User _user;

        public OffersTripPage(User user)
        {
            InitializeComponent();
            _user = user;
            Title = "Welcome, " + user.FirstName + " " + user.LastName;
            ThreadGetTripsOffered();
            GetPassengerCurrentTrip();
            GetTripsOffered();
        }

        void ThreadGetTripsOffered()
        {
            // Thread of query to new posted trips
            if (!stopWatch.IsRunning)
                stopWatch.Start();
            
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (stopWatch.IsRunning && stopWatch.Elapsed.Seconds >= defaultTimespan)
                {
                    Console.WriteLine("Get Trips Offered");
                    Device.BeginInvokeOnMainThread(() => {
                        GetTripsOffered();
                    });

                    stopWatch.Restart();
                }
                return true;
            });
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
                    List<Trip> list = await tripRepository.GetTripsOffered();
                    _tripOfferList = new ObservableCollection<Trip>(list);
                    OfferTrip.ItemsSource = _tripOfferList;
                }
            }
            catch(Exception e)
            {
                await DisplayAlert("Error", "An unexpected error has occurred"+ e.Message, "Ok");
            }
        }

        /*public void OfferTrip_Refreshing(object sender, EventArgs e)
        {
            GetTripsOffered();
            OfferTrip.EndRefresh();
        }*/

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

        async void OfferTrip_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            try
            {
                if (e.CurrentSelection.Count <= 0)
                    return;

                using (TripRepository tripRepository = new TripRepository())
                {

                    var selectedTrip = (e.CurrentSelection[0] as Trip);
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ThreadGetTripsOffered();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (stopWatch.IsRunning)
                stopWatch.Stop();
        }
    }
}
