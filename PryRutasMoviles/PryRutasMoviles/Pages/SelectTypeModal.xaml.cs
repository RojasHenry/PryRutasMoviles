using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

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

        void btnPassenger_Clicked(System.Object sender, System.EventArgs e)
        {
            setResultAction?.Invoke(false);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        void btnDriver_Clicked(System.Object sender, System.EventArgs e)
        {
            setResultAction?.Invoke(true);
            this.Navigation.PopPopupAsync().ConfigureAwait(true);
        }
    }
}
