namespace PryRutasMoviles.Models
{
    public class OffertedTrip
    {
        public OffertedTripDriver OffertedTripDriver { get; set; }
        public string Route { get; set; }
        public decimal Price { get; set; }
        public string MeetingPoint { get; set; }
        public string MeetingTime { get; set; }
        public int SeatsAvailables { get; set; }
        public string State { get; set; }
        public OffertedTripVehicle OffertedTripVehicle { get; set; }
    }
}
