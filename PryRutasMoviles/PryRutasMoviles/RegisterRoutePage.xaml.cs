using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PryRutasMoviles.Entities;
using PryRutasMoviles.Models;
using PryRutasMoviles.Repositories;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PryRutasMoviles
{
    public partial class RegisterRoutePage : ContentPage
    {
        private readonly User _driver;
        private readonly Route route = new Route();
        private string workingPointFlag = string.Empty;
        
        public RegisterRoutePage(User driver)
        {
            InitializeComponent();
            _driver = driver;
            GetLocation();
            tpMeetingTime.Time = DateTime.Now.TimeOfDay;
        }

        #region events

        public async void BtnMeetingPoint_Clicked(object sender, EventArgs e)
        {
            EnableDisableControls(false);
            workingPointFlag = "MeetingPoint";
            if (route.MeetingPoint != null)
                await EditMeetingPoint();
            else
                await SelectMeetingPoint();
        }

        public async void BtnGetTargetPoint_Clicked(object sender, EventArgs e)
        {
            EnableDisableControls(false);
            workingPointFlag = "TargetPoint";
            if (route.TargetPoint != null)
                await EditTargetPoint();
            else
                await SelectTargetPoint();
        }

        public async void BtnPostTrip_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (TripRepository tripRepository = new TripRepository())
                {
                    if (IsValidForm())
                    {
                        var result = await ConfirmPostTrip(this.Navigation);
                        if (result)
                        {
                            EnableDisableActivityIndicator(true);
                            EnableDisableControls(false);
                            var trip = new Trip
                            {
                                TripId = Guid.NewGuid().ToString(),
                                Driver = _driver,
                                TripRoute = new TripRoute
                                {
                                    MeetingPointLongitude = route.MeetingPoint.Pin.Position.Longitude,
                                    MeetingPoitnLatitude = route.MeetingPoint.Pin.Position.Latitude,
                                    MeetingPoitnAddress = route.MeetingPoint.Pin.Address,
                                    TargetPointLongitude = route.TargetPoint.Pin.Position.Longitude,
                                    TargetPointLatitude = route.TargetPoint.Pin.Position.Latitude,
                                    TargetPoitnAddress = route.TargetPoint.Pin.Address
                                },
                                MeetingTime = tpMeetingTime.Time.ToString(),
                                Price = Convert.ToDecimal(txtPrice.Text),
                                SeatsAvailables = Convert.ToInt16(txtSeatsAvailables.Text),
                                State = "Posted"
                            };

                            await tripRepository.AddTrip(trip);
                            EnableDisableActivityIndicator(false);                            
                            await DisplayAlert("Info", "Route posted successfully", "Ok");
                            workingPointFlag = string.Empty;
                            RemovePoint();
                            CleanEntries();
                            EnableDisableControls(true);
                            // {.....}move to next page;

                        }
                    }
                }
            }
            catch
            {
                EnableDisableActivityIndicator(false);
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }            
        }

        public async void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            try
            {
                Position positionSelected = e.Position;

                if (workingPointFlag.Equals("MeetingPoint"))
                {
                    if (map.Pins.Count > 0)
                    {
                        if (route.MeetingPoint != null)
                            map.Pins.Remove(route.MeetingPoint.Pin);
                    }
                    route.MeetingPoint = new MeetingPoint
                    {
                        Pin = new Pin
                        {
                            ClassId = workingPointFlag,
                            Label = workingPointFlag,
                            Position = positionSelected,
                            Address = await GetAddress(positionSelected)
                        },
                    };
                }
                else
                {
                    if (map.Pins.Count > 0)
                    {
                        if (route.TargetPoint != null)
                            map.Pins.Remove(route.MeetingPoint.Pin);
                    }
                    route.TargetPoint = new TargetPoint
                    {
                        Pin = new Pin
                        {
                            ClassId = workingPointFlag,
                            Label = workingPointFlag,
                            Position = positionSelected,
                            Address = await GetAddress(positionSelected)
                        },
                    };
                }

                DrawRoute(route);
            }
            catch
            {
                await DisplayAlert("Error", "An unexpected error has occurred", "Ok");
            }

        }

        public void BtnAcept_Clicked(object sender, EventArgs e)
        {
            map.MapClicked -= Map_MapClicked;
            frameInfo.IsVisible = false;
            layoutButtons.IsVisible = false;
            btnGetMeetingPoint.IsEnabled = true;
            btnGetTargetPoint.IsEnabled = true;
            btnPostTrip.IsEnabled = true;
            EnableDisableControls(true);
        }

        public void BtnCancel_Clicked(object sender, EventArgs e)
        {
            
            RemovePoint();
            map.MapClicked -= Map_MapClicked;
            frameInfo.IsVisible = false;
            layoutButtons.IsVisible = false;
            EnableDisableControls(true);
        }
        #endregion

        #region funtional methods
        private async Task SelectMeetingPoint()
        {
            string response = await DisplayActionSheet("Choose your current location as meeting point?"
                , null, null, "Yes", "No");

            if (response.Equals("Yes"))
            {
                route.MeetingPoint = new MeetingPoint
                {
                    Pin = await GetCurrentPin()
                };

                DrawRoute(route);
            }
            else
            {
                frameInfo.IsVisible = true;
                txtMapMessage.Text = "Select the meeting point on the map";
                map.MapClicked += Map_MapClicked;
            }
        }

        private async Task SelectTargetPoint()
        {
            string response = await DisplayActionSheet("Choose your current location as target point?"
                , null, null, "Yes", "No");

            if (response.Equals("Yes"))
            {
                route.TargetPoint = new TargetPoint
                {
                    Pin = await GetCurrentPin()
                };
                DrawRoute(route);
            }
            else
            {
                frameInfo.IsVisible = true;
                txtMapMessage.Text = "Select the target point on the map";
                map.MapClicked += Map_MapClicked;
            }
        }

        private async Task EditMeetingPoint()
        {
            string response = await DisplayActionSheet("Do you want change meeting point?"
                , null, null, "Yes", "No");

            if (response.Equals("Yes"))
            {
                RemovePoint();
                route.MeetingPoint = null;

                response = await DisplayActionSheet("Choose your current location as meeting point?"
                , null, null, "Yes", "No");

                if (response.Equals("Yes"))
                {
                    route.MeetingPoint = new MeetingPoint
                    {
                        Pin = await GetCurrentPin()
                    };

                    DrawRoute(route);
                }
                else
                {
                    frameInfo.IsVisible = true;
                    txtMapMessage.Text = "Select the meeting point on the map";
                    map.MapClicked += Map_MapClicked;
                }
            }
            else
                EnableDisableControls(true);
        }

        private async Task EditTargetPoint()
        {
            string response = await DisplayActionSheet("Do you want change target point?"
                , null, null, "Yes", "No");

            if (response.Equals("Yes"))
            {
                RemovePoint();
                route.TargetPoint = null;

                response = await DisplayActionSheet("Choose your current location as target point?"
                , null, null, "Yes", "No");

                if (response.Equals("Yes"))
                {
                    route.TargetPoint = new TargetPoint
                    {
                        Pin = await GetCurrentPin()
                    };

                    DrawRoute(route);
                }
                else
                {
                    frameInfo.IsVisible = true;
                    txtMapMessage.Text = "Select the target point on the map";
                    map.MapClicked += Map_MapClicked;
                }
            }
            else
                EnableDisableControls(true);
        }

        private bool IsValidForm()
        {

            if (route.MeetingPoint == null || route.TargetPoint == null)
            {
                DisplayAlert("Alerta", "Please select a route", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(txtPrice.Text) || Convert.ToDouble(txtPrice.Text) == 0)
            {
                DisplayAlert("Alerta", "The price of the trip cannot be zero", "Ok");
                return false;
            }

            if (tpMeetingTime.Time.CompareTo(DateTime.Now.TimeOfDay) < 0)
            {
                DisplayAlert("Alerta", "Meeting time must be greater than current hour", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(txtPrice.Text) || Convert.ToInt16(txtSeatsAvailables.Text) == 0)
            {
                DisplayAlert("Alerta", "The number of seats available cannot be zero", "Ok");
                return false;
            }

            return true;
        }

        private async Task<bool> ConfirmPostTrip(INavigation navigation)
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            void callback(bool didConfirm)
            {
                completionSource.TrySetResult(didConfirm);
            }

            var popup = new DetailRoutePopup(callback, route);
            await navigation.PushPopupAsync(popup);
            return await completionSource.Task;
        }
        #endregion      

        #region common methods
        private void EnableDisableActivityIndicator(bool flagActivityIndicator)
        {
            activity.IsEnabled = flagActivityIndicator;
            activity.IsRunning = flagActivityIndicator;
            activity.IsVisible = flagActivityIndicator;
        }
        private void EnableDisableControls(bool controlFlag) 
        {
            btnGetMeetingPoint.IsEnabled = controlFlag;
            btnGetTargetPoint.IsEnabled = controlFlag;
            btnPostTrip.IsEnabled = controlFlag;
        }
        private void RemovePoint()
        {
            if (map.MapElements.Count > 0)
                map.MapElements.Clear();

            if (workingPointFlag.Equals("MeetingPoint"))
            {
                map.Pins.Remove(route.MeetingPoint.Pin);
                route.MeetingPoint = null;
            }
            else if (workingPointFlag.Equals("TargetPoint"))
            {
                map.Pins.Remove(route.TargetPoint.Pin);
                route.TargetPoint = null;
            }
            else
            {
                map.MapElements.Clear();
                map.Pins.Clear();
                layoutButtons.IsVisible = false;
                frameInfo.IsVisible = false;
                route.MeetingPoint = null;
                route.TargetPoint = null;
                EnableDisableControls(true);
            }
        }

        private void CleanEntries() {
            txtMapMessage.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtSeatsAvailables.Text = string.Empty;
            tpMeetingTime.Time = DateTime.Now.TimeOfDay;
        }

        private async void GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Position position = new Position(location.Latitude, location.Longitude);
                    MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
                    map.MoveToRegion(mapSpan);
                }
            }
            catch { }
        }

        private async Task<Pin> GetCurrentPin()
        {
            return new Pin
            {
                Position = await GetLocationUserAsync(),
                Label = workingPointFlag,
                ClassId = workingPointFlag,
                Address = await GetAddress(await GetLocationUserAsync())
            };
        }

        private async Task<Position> GetLocationUserAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                    return new Position(location.Latitude, location.Longitude);
                else
                    return new Position(0.0, 0.0);
            }
            catch
            {
                return new Position(0.0, 0.0);
            }
        }

        private async Task<string> GetAddress(Position position)
        {
            Geocoder geocoder = new Geocoder();

            try
            {
                IEnumerable<string> possibleAddresses =
                    await geocoder.GetAddressesForPositionAsync(position);
                return possibleAddresses.FirstOrDefault();
            }
            catch
            {
                return "It was not possible to get address";
            }
        }

        private Position ComputeCentroid(Route route)
        {
            double latitude = (route.MeetingPoint.Pin.Position.Latitude
                + route.TargetPoint.Pin.Position.Latitude) / 2;
            double longitude = (route.MeetingPoint.Pin.Position.Longitude +
                route.TargetPoint.Pin.Position.Longitude) / 2;

            return new Position(latitude, longitude);
        }

        private void DrawRoute(Route route)
        {
            Pin selectedPin;
            MapSpan mapSpan;

            if (workingPointFlag.Equals("MeetingPoint"))
                selectedPin = route.MeetingPoint.Pin;
            else
                selectedPin = route.TargetPoint.Pin;

            layoutButtons.IsVisible = true;
            txtMapMessage.Text = $"{workingPointFlag.Replace("Point", " point")} selected: {selectedPin.Address}";
            frameInfo.IsVisible = true;

            if (route.MeetingPoint != null && route.TargetPoint != null)
            {
                if (route.MeetingPoint.Pin.Address.Equals(route.TargetPoint.Pin.Address))
                {
                    DisplayAlert("Alert", "Meeting Point and target point are the same", "Try Again");
                    workingPointFlag = string.Empty;
                    RemovePoint();
                    return;
                }

                Polyline polyLine = new Polyline
                {
                    StrokeColor = Color.Blue,
                    StrokeWidth = 12,
                    Geopath =
                    {
                        route.MeetingPoint.Pin.Position,
                        route.TargetPoint.Pin.Position,
                    }
                };

                var zoomLevel = 15; // between 1 and 18
                var latLongDeg = 360 / (Math.Pow(2, zoomLevel));
                Position center = ComputeCentroid(route);
                mapSpan = new MapSpan(center, latLongDeg, latLongDeg);
                map.MapElements.Add(polyLine);
            }
            else
                mapSpan = new MapSpan(selectedPin.Position, 0.01, 0.01);

            map.MoveToRegion(mapSpan);
            map.Pins.Add(selectedPin);
        }
        #endregion
    }
}
