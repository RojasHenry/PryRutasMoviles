using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class TripWaitingRoomPage : ContentPage
    {
        private static Stopwatch stopWatch = new Stopwatch();
        private const int defaultTimespan = 10;

        private ObservableCollection<User> _passengersList;
        public Trip Trip { get; set; }
        
        public TripWaitingRoomPage(Trip trip)
        {
            InitializeComponent();
            Trip = trip;            
            GetPassengersWaiting(Trip);
            EnableDisableTripButtons(Trip.State);
            BindingContext = this;
            // only if Trip is posted 
            if (trip.State.Equals("Posted"))
            {
                ThreadStateTrip(trip);
            }
            
        }

        void ThreadStateTrip(Trip trip)
        {
            // Thread of query to new passengers
            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
            }

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (stopWatch.IsRunning && stopWatch.Elapsed.Seconds >= defaultTimespan)
                {
                    Console.WriteLine("Consultando.....");
                    Device.BeginInvokeOnMainThread(() => {
                        GetPassengersWaiting(trip);
                    });

                    stopWatch.Restart();
                }
                return true;
            });
        }
                
        private async void GetPassengersWaiting(Trip trip)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    _passengersList = new ObservableCollection<User>();
                    var tripInDB = await tripRepository.GetTripById(trip.TripId);
                    if (tripInDB.Passengers == null) 
                    {
                        passengersListView.ItemsSource = _passengersList;
                        return;
                    }                        

                    tripInDB.Passengers
                        .Where(p => p.State)
                        .ToList()
                        .ForEach(p => _passengersList.Add(p));
                    passengersListView.ItemsSource = _passengersList;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An unexpected error has occurred"+ ex.Message, "Ok");
            }
        }

        private void PassengersListView_Refreshing(object sender, EventArgs e)
        {
            GetPassengersWaiting(Trip);
            passengersListView.EndRefresh();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }        

        private async void BtnInTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_passengersList.Count ==0)
                {
                    await DisplayAlert("Alert", "There's not passengers yet", "Ok");
                    return;
                }

                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.UpdateStateTrip("OnWay", Trip.TripId);
                    EnableDisableTripButtons("OnWay");
                    stopWatch.Stop();
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        private async void BtnCancelTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.CancelTrip(Trip.TripId);
                    await Navigation.PopAsync();
                    stopWatch.Stop();
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        private async void BtnFinishTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.FinishTrip(Trip.TripId);
                    await Navigation.PopAsync();
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        private async void RemovePassenger_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    var tripState = await tripRepository.GetStatusTrip(Trip.TripId);
                    if (tripState.Equals("OnWay"))
                    {
                        await DisplayAlert("Alert", "The trip is on way, please Finish the trip by passenger", "Ok");
                        return;
                    }
                    var passenger = (sender as MenuItem).CommandParameter as User;
                    await tripRepository.RemovePassenger(passenger, Trip.TripId);
                    _passengersList.Remove(passenger);
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }            
        }

        private async void BtnFinishPassengerTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    var tripState = await tripRepository.GetStatusTrip(Trip.TripId);
                    if (tripState.Equals("Posted"))
                    {
                        await DisplayAlert("Alert", "The trip has not started yet", "Ok");
                        return;
                    }
                    var passenger = (sender as Button).CommandParameter as User;
                    await tripRepository.FinishPassengerTrip(passenger, Trip.TripId);
                    _passengersList.Remove(passenger);
                    if (_passengersList.Count==0)
                    {
                        await tripRepository.FinishTrip(Trip.TripId);
                        await Navigation.PopAsync();
                        stopWatch.Stop();
                    }
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        private void EnableDisableTripButtons(string tripState) 
        {
            switch (tripState)
            {
                case "Initial":
                case "Posted":
                case "Canceled":
                case "Finished":
                    btnInitTrip.IsVisible = true;
                    btnCancelTrip.IsVisible = true;
                    btnFinishTrip.IsVisible = false;
                    break;
                case "OnWay":
                    btnInitTrip.IsVisible = false;
                    btnCancelTrip.IsVisible = true;
                    btnFinishTrip.IsVisible = true;
                    break;                
            }
            
        }

        private void PassengersListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            passengersListView.SelectedItem = null;
        }
    }
}
