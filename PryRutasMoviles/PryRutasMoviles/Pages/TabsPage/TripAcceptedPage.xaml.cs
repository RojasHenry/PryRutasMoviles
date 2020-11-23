using System;
using System.Diagnostics;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class TripAcceptedPage : ContentPage
    {
        private static Stopwatch stopWatch = new Stopwatch();
        private const int defaultTimespan = 10;
        
        public Trip Trip { get; set; }
        public User User { get; set; }

        public TripAcceptedPage(Trip trip, User user)
        {
            InitializeComponent();
            Trip = trip;
            User = user;
            ThreadStartTrip();
            BindingContext = this;
        }

        void ThreadStartTrip()
        {
            // Thread of query to new posted trips
            if (!stopWatch.IsRunning)
                stopWatch.Start();

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (stopWatch.IsRunning && stopWatch.Elapsed.Seconds >= defaultTimespan)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        DetectChangeStatusTrip();
                    });

                    stopWatch.Restart();
                }
                return true;
            });
        }

        private async void DetectChangeStatusTrip() 
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var tripState = await tripRepository.GetStatusTrip(Trip.TripId);

                switch (tripState)
                {
                    case "OnWay":
                        txtwarning.Text = "Please wait for " + Trip.Driver.FirstName.Trim() + " to charge you and finish the trip.";
                        txtwarning.IsVisible = true;
                        BtnCancelTrip.IsVisible = false;
                        break;

                    case "Finished":
                    case "Canceled":
                        await Navigation.PopAsync();
                        break;
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void BtnCancelTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.RemovePassenger(User, Trip.TripId);

                    await Navigation.PopAsync();
                }
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", "An unexpected error has occurred" + exc.Message, "Ok");
            }            
        }
    }
}
