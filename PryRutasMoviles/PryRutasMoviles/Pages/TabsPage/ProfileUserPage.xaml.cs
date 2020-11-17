using PryRutasMoviles.Models;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class ProfileUserPage : ContentPage
    {
        public User User { get; set; }
        public ProfileUserPage(User user)
        {
            InitializeComponent();
            User = user;
            BindingContext = this;
        }
    }
}
