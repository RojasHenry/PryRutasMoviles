using System;
using System.Collections.Generic;
using System.Text;

namespace PryRutasMoviles.Models
{
    public class Route
    {
        public string Position { get; set; }
        public decimal Price { get; set; }
        public string MeetingPoint { get; set; }
        public string MeetingTime { get; set; }
        public int SeatsAvailables { get; set; }        
    }
}
