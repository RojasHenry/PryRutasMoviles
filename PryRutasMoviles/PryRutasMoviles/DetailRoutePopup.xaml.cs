using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace PryRutasMoviles
{
    public partial class DetailRoutePopup : PopupPage
    {
        private readonly Action<bool> setResultAction;

        public DetailRoutePopup(Action<bool> setResultAction)
        {
            InitializeComponent();
            this.setResultAction = setResultAction;
        }

        public void CancelAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(false);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }

        public void ConfirmAttendanceClicked(object sender, EventArgs e)
        {
            setResultAction?.Invoke(true);
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }
    }
}
