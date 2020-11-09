using System;
using System.Collections.Generic;

namespace PryRutasMoviles.Models
{
    public class Trip
    {
        public string TripId { get; set; }
        public User Driver { get; set; }
        public List<User> Passengers { get; set; }
        public string MeetingDate { get; set; }
        public string MeetingTime { get; set; }
        public string FullMeetingDate { get { return $"{MeetingDate} {MeetingTime}"; } }
        public int SeatsAvailables { get; set; }
        public decimal Price { get; set; }
        public TripRoute TripRoute { get; set; }
        public string State { get; set; } //Offered-full-In way
        
    }
}
