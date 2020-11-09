using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using Xamarin.Forms;

namespace PryRutasMoviles
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());

            //MainPage = new NavigationPage(new TripAcceptedPage(new Models.Trip()
            //{
            //    Driver = new User()
            //    {
            //        FirstName = "Sebastian",
            //        LastName = "Reza",
            //        Vehicle = new Vehicle()
            //        {
            //            Brand = "Porche",
            //            Color = "Green",
            //            Registration = "PBC-154",
            //            State = true,
            //            Year = "2020"
            //        }
            //    },
            //    TripRoute = new Models.TripRoute()
            //    {
            //        MeetingPoitnAddress = "Colon y 9 de octubre",
            //        TargetPoitnAddress = "Conocoto"

            //    },
            //    SeatsAvailables = 2,
            //    MeetingTime = "14:00",
            //    Price = 1,

            //}, new User()));

            //MainPage = new NavigationPage(new TripWaitingRoomPage(new Models.Trip()
            //{
            //    TripRoute = new Models.TripRoute()
            //    {
            //        MeetingPoitnAddress = "Colon y 9 de octubre",
            //        TargetPoitnAddress = "Conocoto"

            //    },
            //    SeatsAvailables = 2,
            //    MeetingTime = "14:00",
            //    Price = 1,

            //}));
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
