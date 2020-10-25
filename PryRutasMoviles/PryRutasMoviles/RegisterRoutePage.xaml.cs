using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PryRutasMoviles
{
    public partial class RegisterRoutePage : ContentPage
    {
        public ObservableCollection<Pin> pinslist = new ObservableCollection<Pin>();
        bool isPuntoEnc = false;

        Pin pin;

        public RegisterRoutePage()
        {
            InitializeComponent();
            map.ItemsSource = pinslist;
            GetLocation();

            txtPrice.Text = "0,00";
            timeHour.Time = DateTime.Now.TimeOfDay;

        }

        public async void GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    Position position = new Position(location.Latitude, location.Longitude);
                    MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
                    map.MoveToRegion(mapSpan);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public async void map_MapClicked(System.Object sender, Xamarin.Forms.Maps.MapClickedEventArgs e)
        {
            Position pos = e.Position;

            Console.WriteLine(pos.Longitude +" "+pos.Latitude);

            pin = new Pin
            {
                Position = pos,
                Label = (isPuntoEnc) ? "Punto de encuentro" : "Destino",
                ClassId = (isPuntoEnc) ? "PE" : "PD"
            };

            stlbtns.IsVisible = true;

            Geocoder geocoder = new Geocoder();

            IEnumerable<string> possibleAddresses = await geocoder.GetAddressesForPositionAsync(pos);
            string address = possibleAddresses.FirstOrDefault();
            txtmsg.Text = "Ubicación escogida: " + address;
            frmMsg.IsVisible = true;

            if(pinslist.Count < 2)
                pinslist.Add(pin);

            if (pinslist.Count == 2)
            {
                Polyline polyline = new Polyline
                {
                    StrokeColor = Color.Blue,
                    StrokeWidth = 12,
                    Geopath =
                        {
                            pinslist[0].Position,
                            pinslist[1].Position
                        }
                };
                var zoomLevel = 13; // pick a value between 1 and 18
                var latlongdeg = 360 / (Math.Pow(2, zoomLevel));
                Position center = computeCentroid(pinslist);
                MapSpan mapSpan = new MapSpan(center, latlongdeg, latlongdeg);
                map.MoveToRegion(mapSpan);
                map.MapElements.Add(polyline);
            }
            else
            {
                MapSpan mapSpan = new MapSpan(pin.Position, 0.01, 0.01);
                map.MoveToRegion(mapSpan);
            }

        }

        async void btnpuntoEn_Clicked(System.Object sender, System.EventArgs e)
        {
            isPuntoEnc = true;
            if (pinslist.Count == 2 || hadChossen(isPuntoEnc))
            {
                await EditRouteLocation(isPuntoEnc);
            }
            else
            {
                await ShowMessageRouteLocations(isPuntoEnc);
            }
            
        }

        async void btndestino_Clicked(System.Object sender, System.EventArgs e)
        {
            isPuntoEnc = false;
            if (pinslist.Count == 2 || hadChossen(isPuntoEnc))
            {
                
                await EditRouteLocation(isPuntoEnc);
            }
            else
            {
                await ShowMessageRouteLocations(isPuntoEnc);
            }

        }

        private async Task ShowMessageRouteLocations(bool isPuntEncuentro)
        {
            string action = await DisplayActionSheet(
                (isPuntEncuentro)?"¿Desea escoger como punto de encuentro su ubicación actual?":"¿Desea escoger como destino su ubicación actual?"
                , null, null, "Si", "No");
            if (action.Equals("Si"))
            {
                Position actualPos = await GetLocationUserAsync();

                pin = new Pin
                {
                    Position = actualPos,
                    Label = (isPuntEncuentro) ? "Punto de encuentro" : "Destino",
                    ClassId = (isPuntEncuentro) ? "PE":  "PD"
                };

                stlbtns.IsVisible = true;

                Geocoder geocoder = new Geocoder();

                IEnumerable<string> possibleAddresses = await geocoder.GetAddressesForPositionAsync(actualPos);
                string address = possibleAddresses.FirstOrDefault();
                txtmsg.Text = "Ubicación escogida: " + address;
                frmMsg.IsVisible = true;

                pinslist.Add(pin);
                if (pinslist.Count == 2)
                {
                    Polyline polyline = new Polyline
                    {
                        StrokeColor = Color.Blue,
                        StrokeWidth = 12,
                        Geopath =
                        {
                            pinslist[0].Position,
                            pinslist[1].Position
                        }
                    };

                    var zoomLevel = 13; // pick a value between 1 and 18
                    var latlongdeg = 360 / (Math.Pow(2, zoomLevel));
                    Position center = computeCentroid(pinslist);
                    MapSpan mapSpan = new MapSpan(center, latlongdeg, latlongdeg);
                    map.MoveToRegion(mapSpan);
                    map.MapElements.Add(polyline);
                }
                else
                {
                    MapSpan mapSpan = new MapSpan(pin.Position, 0.01, 0.01);
                    map.MoveToRegion(mapSpan);
                }
            }
            else
            {
                map.MapClicked += map_MapClicked;
                frmMsg.IsVisible = true;
                txtmsg.Text = (isPuntEncuentro) ? "Seleccione el punto de encuentro en el mapa.":"Seleccione el punto de destino en el mapa.";

            }
        }

        private async Task EditRouteLocation(bool isPuntoEncuentro)
        {
            string action = await DisplayActionSheet(
                (isPuntoEncuentro) ? "¿Desea editar el punto de encuentro escogido?" : "¿Desea editar el destino escogido?",
                null, null, "Si", "No");

            if (action.Equals("Si"))
            {
                foreach (Pin pin in pinslist)
                {
                    if (pin.ClassId.Equals((isPuntoEncuentro) ? "PE" : "PD"))
                    {
                        pinslist.Remove(pin);
                        break;
                    }
                        
                }

                map.MapElements.Clear();
                await ShowMessageRouteLocations(isPuntoEnc);
            }
        }

        public bool hadChossen(bool isPuntoEnc)
        {
            foreach (Pin pin in pinslist)
            {
                if (pin.ClassId.Equals((isPuntoEnc) ? "PE" : "PD"))
                    return true;
            }
            return false;
        }

        public async Task<Position> GetLocationUserAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    Position position = new Position(location.Latitude, location.Longitude);
                    return position;
                }
                else
                {
                    return new Position(0.0, 0.0);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                return new Position(0.0, 0.0);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                return new Position(0.0, 0.0);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                return new Position(0.0, 0.0);
            }
            catch (Exception ex)
            {
                // Unable to get location
                return new Position(0.0, 0.0);
            }
        }

        void btnAceptar_Clicked(System.Object sender, System.EventArgs e)
        {
            if (pinslist.Count == 2)
            {
                frmMsg.IsVisible = false;
            }
            else
            {
                map.MapClicked -= map_MapClicked;
                isPuntoEnc = false;
                frmMsg.IsVisible = false;
            }

            stlbtns.IsVisible = false;
        }

        void btnCancelar_Clicked(System.Object sender, System.EventArgs e)
        {
            if (pinslist.Count == 2)
            {
                pinslist.Remove(pin);
                map.MapElements.Clear();
                map.MapClicked -= map_MapClicked;
                isPuntoEnc = false;
                frmMsg.IsVisible = false;
                stlbtns.IsVisible = false;
            }
            else
            {
                pinslist.Remove(pin);
                map.MapClicked -= map_MapClicked;
                isPuntoEnc = false;
                frmMsg.IsVisible = false;
                stlbtns.IsVisible = false;
            }
            
        }

        private Position computeCentroid(ObservableCollection<Pin> points)
        {
            double latitude = 0;
            double longitude = 0;
            int n = points.Count;

            foreach(Pin point in points)
            {
                latitude += point.Position.Latitude;
                longitude += point.Position.Longitude;
            }

            return new Position(latitude / n, longitude / n);
        }

        async void btnOfertar_Clicked(System.Object sender, System.EventArgs e)
        {
            /*if (isValidaForm())
            {*/
                var result = await ConfirmConferenceAttendance(this.Navigation);
           /* }*/
        }

        private bool isValidaForm()
        {
            bool isValid = true;

            if(pinslist.Count != 2 || txtPrice.Text.Equals(""))
            {
                DisplayAlert("Alerta", "Complete todos los campos.", "OK");
                return false;
            }

            if (pinslist.Count != 2)
            {
                DisplayAlert("Alerta", "Complete toda su ruta.", "OK");
                return false;
            }

            double precio = Convert.ToDouble(txtPrice.Text);

            if (precio == 0)
            {
                DisplayAlert("Alerta", "Complete el precio de la ruta.", "OK");
                return false;
            }

            if(timeHour.Time.CompareTo(DateTime.Now.TimeOfDay) < 0)
            {
                DisplayAlert("Alerta", "Hora menor a la actual.", "OK");
                return false;
            }

            return isValid;
        }

        public static async Task<bool> ConfirmConferenceAttendance(INavigation navigation)
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            void callback(bool didConfirm)
            {
                completionSource.TrySetResult(didConfirm);
            }

            var popup = new DetailRoutePopup(callback);

            await navigation.PushPopupAsync(popup);

            return await completionSource.Task;
        }
    }
}
