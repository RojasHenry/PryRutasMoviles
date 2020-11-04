using System;
using System.Collections.Generic;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class TripAcceptedPage : ContentPage
    {
        readonly Trip Trip;
        User user;
        public TripAcceptedPage(Trip TripAccepted, User userActual)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Trip = TripAccepted;
            user = userActual;
            BindingContext = this;
        }

        async void btnCancelTrip_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    await tripRepository.RemovePassenger(user, Trip.TripId);

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
