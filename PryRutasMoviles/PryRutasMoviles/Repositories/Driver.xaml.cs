using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Repositories
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Driver : TabbedPage
    {
        public Driver()
        {
            InitializeComponent();
        }
    }
}