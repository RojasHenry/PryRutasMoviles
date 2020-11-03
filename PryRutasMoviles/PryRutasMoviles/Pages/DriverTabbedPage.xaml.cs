using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriverTabbedPage : TabbedPage
    {
        public DriverTabbedPage(User user, bool isDriver)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            Title = "Bienvenido, "+user.FirstName + " "+ user.LastName;

            if (isDriver)
            {
                NavigationPage registerRoute = new NavigationPage(new RegisterDriverRoutePage());
                registerRoute.Title = "Registro de Ruta";

                NavigationPage tripsDriver = new NavigationPage(new MyTripDriverPage());
                tripsDriver.Title = "Mis Viajes";

                Children.Add(registerRoute);
                Children.Add(tripsDriver);
            }
            else
            {
                NavigationPage registerRoute = new NavigationPage(new OffersTripPage());
                registerRoute.Title = "Oferta de Rutas";

                NavigationPage tripsDriver = new NavigationPage(new MyTripDriverPage());
                tripsDriver.Title = "Mis Viajes";

                Children.Add(registerRoute);
                Children.Add(tripsDriver);
            }

        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProfileUserPage());
        }

    }
}
