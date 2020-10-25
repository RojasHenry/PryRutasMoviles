using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Extensions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarYearList : ContentPage
    {
        public CarYearList()
        {
            InitializeComponent();
            yearListView.ItemsSource = GetYears(2000, 2020);
        }

        public ListView CarYears { get { return yearListView; } }

        private List<string> GetYears(int minYear,int maxYear) 
        {
            List<string> result = new List<string>();
            for (int i = minYear; i <= maxYear; i++)
                result.Add(i.ToString());
            return result;
        }
    }
}