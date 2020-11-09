namespace PryRutasMoviles.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public Vehicle Vehicle { get; set; }
        public bool IsFromSocialNetworks { get; set; }
        public bool State { get; set; }
    }
}
