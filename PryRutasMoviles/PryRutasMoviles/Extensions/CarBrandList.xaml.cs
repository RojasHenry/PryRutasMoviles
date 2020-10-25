using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Extensions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarBrandList : ContentPage
    {
        public CarBrandList()
        {
            InitializeComponent();
            brandListView.ItemsSource = new List<string>{
                "Toyota"
                ,"KIA"
                ,"Chevi"
            };
        }

        public ListView CarBrands { get { return brandListView; } }
    }
}