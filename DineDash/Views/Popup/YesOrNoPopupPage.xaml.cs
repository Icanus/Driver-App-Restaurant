using DineDash.Models;
using DineDash.Utilities;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YesOrNoPopupPage : PopupPage
    {
        public event EventHandler<EventArgs> OperationCompleted;
        public OrderParameter order { get; set; }
        public YesOrNoPopupPage(string title, string message, OrderParameter orderParameter)
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
            AttachEventHandlers();
            order = orderParameter;
            ConfirmationTitle.Text = title;
            ConfirmationContentLabel.Text = message + $"\n Order Id: ORD-000{order.Id}";
            //this.BackgroundColor = AppColor.VioletBackgroundOverlay;
        }

        void AttachEventHandlers()
        {
            AttachCancelButtonTappedHandler();
            AttachLogoutButtonTappedHandler();
        }

        void AttachCancelButtonTappedHandler()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += async (sender, e) =>
            {
                var v = sender as View;
                //await v.AnimateBackgroundColorAsync();
                v.IsEnabled = false;
                await PopupNavigation.Instance.PopAsync();
                v.IsEnabled = true;

            };
            CancelButton.GestureRecognizers.Add(tapGestureRecognizer);
        }

        void AttachLogoutButtonTappedHandler()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += async (sender, e) =>
            {
                var v = sender as View;
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingContent.IsVisible = true;
                });
                await Task.Delay(300);
                //await v.AnimateBackgroundColorAsync();

                if (CancelButton.IsEnabled)
                {
                    CancelButton.IsEnabled = false;
                    var res = await JsonWebApiAction.UpdateOrderStatus(order.OrderId, "OnTheWay");
                    await PopupNavigation.Instance.PopAsync();

                    OperationCompleted?.Invoke(this, EventArgs.Empty);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LoadingContent.IsVisible = false;
                    });
                }
            };
            LogoutButton.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}