using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Extensions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarColorList : ContentPage
    {
        public CarColorList()
        {
            InitializeComponent();
            colorListView.ItemsSource = new List<string>
            {
                "white",
                "black",
                "red",
                "green",
            };
        }           
        public ListView CarColors { get { return colorListView; } }
    }
}

    
