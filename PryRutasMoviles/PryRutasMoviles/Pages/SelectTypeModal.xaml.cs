using System;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;

namespace PryRutasMoviles.Pages
{
    public partial class SelectTypeModal : PopupPage
    {
        private readonly Action<bool> setResultAction;

        public SelectTypeModal(Action<bool> setResultAction)
        {
            InitializeComponent();

            this.setResultAction = setResultAction;
            CloseWhenBackgroundIsClicked = false;
        }

        void btnPassenger_Clicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(false);
            Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        void btnDriver_Clicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(true);
            Navigation.PopPopupAsync().ConfigureAwait(true);
        }
    }
}
