using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class TripWaitingRoomPage : ContentPage
    {
        readonly ObservableCollection<User> userWaiting = new ObservableCollection<User>();
        Trip actualTrip;
        public TripWaitingRoomPage(string tripId)
        {
            InitializeComponent();

            GetActualTrip(tripId);
            GetUsersWaiting(tripId);
        }

        async private void GetUsersWaiting(string TripId)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    userWaiting.Clear();
                    Trip Trip = await tripRepository.GetTrip(TripId);
                    Trip.Passengers.ForEach(pas => userWaiting.Add(pas));
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        async private void GetActualTrip(string TripId)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    actualTrip = await tripRepository.GetTrip(TripId);
                    BindingContext = this;
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        async void btnInitTrip_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.UpdateStateTrip("OnWay", actualTrip.TripId);

                    GetUsersWaiting(actualTrip.TripId);
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }

        async void btnCancelTrip_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.UpdateStateTrip("Canceled", actualTrip.TripId);
                    await Navigation.PopAsync();
                }
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }
        }
    }
}
