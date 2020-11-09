using System;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class TripAcceptedPage : ContentPage
    {        
        public Trip Trip { get; set; }
        public User User { get; set; }

        public TripAcceptedPage(Trip trip, User user)
        {
            InitializeComponent();
            Trip = trip;
            User = user;            
            BindingContext = this;
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
