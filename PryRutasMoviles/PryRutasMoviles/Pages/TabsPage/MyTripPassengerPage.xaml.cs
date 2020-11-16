using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class MyTripPassengerPage : ContentPage
    {
        private ObservableCollection<Trip> _passengerTripsList;
        private User _passenger;
        public MyTripPassengerPage(User passenger)
        {
            InitializeComponent();
            _passenger = passenger;
            GetPassengerFinishedTrips();
        }

        public async void GetPassengerFinishedTrips()
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var passengerFinishedTrips = await tripRepository.GetPassengerFinishedTrips(_passenger);
                _passengerTripsList = new ObservableCollection<Trip>(passengerFinishedTrips);
                passengerTripsListView.ItemsSource = _passengerTripsList;
            }
        }

        private void PassengerTripsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            passengerTripsListView.SelectedItem = null;
        }

        private void PassengerTripsListView_Refreshing(object sender, System.EventArgs e)
        {
            GetPassengerFinishedTrips();
            passengerTripsListView.EndRefresh();
        }
    }
}
