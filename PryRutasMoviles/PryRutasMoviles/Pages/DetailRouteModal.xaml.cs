using System;
using PryRutasMoviles.Entities;
using PryRutasMoviles.Models;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PryRutasMoviles.Pages
{
    public partial class DetailRouteModal : PopupPage
    {
        private readonly Action<bool> setResultAction;
        private readonly Route _route;
        public Trip TripIn { get; set; }

        public DetailRouteModal(Action<bool> setResultAction, Route route)
        {
            InitializeComponent();
            _route = route;
            ShowResume(_route);
            this.setResultAction = setResultAction;
        }

        public DetailRouteModal(Action<bool> setResultAction, Trip trip)
        {
            InitializeComponent();
            _route = new Route()
            {
                MeetingPoint = new Pin()
                {
                    Position = new Position(trip.TripRoute.MeetingPoitnLatitude, trip.TripRoute.MeetingPointLongitude),
                    Label = "MeetingPoint",
                    Address= trip.TripRoute.MeetingPoitnAddress
                },
                TargetPoint = new Pin()
                {
                    Position = new Position(trip.TripRoute.TargetPointLatitude, trip.TripRoute.TargetPointLongitude),
                    Label = "Target",
                    Address = trip.TripRoute.TargetPoitnAddress
                }
            };
            TripIn = trip;
            ShowResume(_route);
            this.setResultAction = setResultAction;
            BindingContext = this;
        }

        public void CancelAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(false);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        public void ConfirmAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(true);
            this.Navigation.PopPopupAsync().ConfigureAwait(true);
        }

        private void ShowResume(Route route)
        {
            MapSpan mapSpan;

            Polyline polyline = new Polyline
            {
                StrokeColor = Color.Blue,
                StrokeWidth = 12,
                Geopath =
                        {
                            route.MeetingPoint.Position,
                            route.TargetPoint.Position,
                        }
            };

            var zoomLevel = 15;
            var latlongdeg = 360 / (Math.Pow(2, zoomLevel));
            Position center = ComputeCentroid(route);
            mapSpan = new MapSpan(center, latlongdeg, latlongdeg);
            map.MoveToRegion(mapSpan);
            map.Pins.Add(route.MeetingPoint);
            map.Pins.Add(route.TargetPoint);
            map.MapElements.Add(polyline);
            Resume.Text = $"Meeting Point: {route.MeetingPoint.Address}\n" +
                $"Target Point: {route.TargetPoint.Address}";
        }

        private Position ComputeCentroid(Route route)
        {
            double latitude = (route.MeetingPoint.Position.Latitude
                + route.TargetPoint.Position.Latitude) / 2;
            double longitude = (route.MeetingPoint.Position.Longitude +
                route.TargetPoint.Position.Longitude) / 2;

            return new Position(latitude, longitude);
        }
    }
}
