
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PryRutasMoviles.Extensions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenericEntryNumberCell : ViewCell
    {
        public static readonly BindableProperty PlaceHolderProperty =
           BindableProperty.Create("Placeholder", typeof(string), typeof(GenericCell));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(GenericCell));

        public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create("IsPassword", typeof(bool), typeof(GenericCell));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        public GenericEntryNumberCell()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}