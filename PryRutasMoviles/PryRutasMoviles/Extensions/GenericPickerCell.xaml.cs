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
    public partial class GenericPickerCell : ViewCell
    {
        public GenericPickerCell()
        {
            InitializeComponent();
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}