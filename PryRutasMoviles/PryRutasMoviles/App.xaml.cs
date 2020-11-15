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
            //MainPage = new NavigationPage(new ProfileUserPage(new User()));
            //MainPage = new NavigationPage(new MyTripPassengerPage(new User { 
            //    Email = "jsrezam@gmail.com"                
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
