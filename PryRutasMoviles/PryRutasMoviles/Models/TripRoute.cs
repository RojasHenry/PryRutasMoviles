namespace PryRutasMoviles.Models
{
    public class TripRoute
    {
        public double MeetingPointLongitude { get; set; }
        public double MeetingPoitnLatitude { get; set; }
        public string MeetingPoitnAddress { get; set; }
        public double TargetPointLongitude { get; set; }
        public double TargetPointLatitude { get; set; }
        public string TargetPoitnAddress { get; set; }
    }
}
