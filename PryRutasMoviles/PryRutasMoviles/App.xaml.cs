﻿using PryRutasMoviles.Models;
using PryRutasMoviles.Pages.TabsPage;
using Xamarin.Forms;

namespace PryRutasMoviles
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());            
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
