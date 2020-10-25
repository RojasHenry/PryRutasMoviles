using System.Collections.Generic;

namespace PryRutasMoviles.Models
{
    public class User
    {
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Vehicle Vehicle { get; set; }
        public bool State { get; set; }
    }
}
