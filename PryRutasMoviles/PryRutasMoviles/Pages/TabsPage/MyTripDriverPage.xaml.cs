using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class MyTripDriverPage : ContentPage
    {
        private ObservableCollection<Trip> _driverTripsList;
        private User _driver;
        public MyTripDriverPage(User driver)
        {
            InitializeComponent();
            _driver = driver;
            Title = "Welcome, " + driver.FirstName + " " + driver.LastName;
            GetDriverFinishedTrips();
        }

        public async void GetDriverFinishedTrips()
        {
            using (TripRepository tripRepository = new TripRepository())
            {
                var driverFinishedTrips = await tripRepository.GetDriverFinishedTrips(_driver);
                _driverTripsList = new ObservableCollection<Trip>(driverFinishedTrips);
                driverTripsListView.ItemsSource = _driverTripsList;
            }
        }

        private void DriverTripsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            driverTripsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetDriverFinishedTrips();
        }
    }
}
