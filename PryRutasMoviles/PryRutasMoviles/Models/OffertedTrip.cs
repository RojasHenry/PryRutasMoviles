using System.Collections.Generic;

namespace PryRutasMoviles.Models
{
    public class OffertedTrip
    {
        public int OffertedTripId { get; set; }
        public User Driver { get; set; }                
        public List<User> Passenger { get; set; }
        public string State { get; set; } //Offered-full-In way
        public Route RouteTrip { get; set; }
    }
}
