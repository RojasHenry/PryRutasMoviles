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
    public partial class GenericCell : ViewCell
    {
        public static readonly BindableProperty PlaceHolderProperty =
            BindableProperty.Create("Placeholder", typeof(string), typeof(GenericCell));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public GenericCell()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}