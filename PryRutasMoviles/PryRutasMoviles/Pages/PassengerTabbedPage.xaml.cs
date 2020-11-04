using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassengerTabbedPage : TabbedPage
    {
        User userActual;
        public PassengerTabbedPage(User user)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            Title = "Bienvenido, " + user.FirstName + " " + user.LastName;

            NavigationPage offerTrips = new NavigationPage(new OffersTripPage(user));
            offerTrips.Title = "Oferta de Rutas";

            NavigationPage tripsPassenger = new NavigationPage(new MyTripPassengerPage());
            tripsPassenger.Title = "Mis Viajes";

            Children.Add(offerTrips);
            Children.Add(tripsPassenger);
            userActual = user;
        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProfileUserPage(userActual));
        }
    }
}
