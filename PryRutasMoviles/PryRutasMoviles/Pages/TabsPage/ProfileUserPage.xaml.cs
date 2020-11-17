using System;
using System.Collections.Generic;
using PryRutasMoviles.Models;
using Xamarin.Forms;

namespace PryRutasMoviles.Pages.TabsPage
{
    public partial class ProfileUserPage : ContentPage
    {
        
        public ProfileUserPage(User userActual)
        {
            InitializeComponent();

        
            

            if (userActual.Vehicle != null)
            {
                // tiene vehiculo
            }
            else
            {
                // no tiene vehiculo
            }
        }
    }
}
