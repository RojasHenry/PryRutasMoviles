using System;
using System.Collections.ObjectModel;
using PryRutasMoviles.Entities;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PryRutasMoviles
{
    public partial class DetailRoutePopup : PopupPage
    {
        private readonly Action<bool> setResultAction;
        private Route _route;

        public DetailRoutePopup(Action<bool> setResultAction, Route route)
        {
            InitializeComponent();
            _route = route;
            ShowResume(_route);
            this.setResultAction = setResultAction;
        }

        public void CancelAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(false);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        public void ConfirmAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(true);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
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
