using DineDash.Models;
using DineDash.Utilities;
using DineDash.ViewModels;
using DineDash.Views;
using DineDash.Views.Popup;
using GoogleApi.Entities.Search.Video.Common.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OngoingOrdersView : ContentView
    {
        bool isPressedAlready = false;
        public OrderParameter OngoingOrder
        {
            get => (OrderParameter)GetValue(OngoingOrderProperty);
            set => SetValue(OngoingOrderProperty, value);
        }

        public static readonly BindableProperty OngoingOrderProperty =
            BindableProperty.Create("OngoingOrder", typeof(OrderParameter), typeof(OngoingOrdersView), new OrderParameter());

        public static readonly BindableProperty ItemTappedProperty =
            BindableProperty.Create(nameof(ItemTapped), typeof(ICommand), typeof(OngoingOrdersView));

        public ICommand ItemTapped
        {
            get => (ICommand)GetValue(ItemTappedProperty);
            set => SetValue(ItemTappedProperty, value);
        }
        public OngoingOrdersView()
        {
            InitializeComponent();
            ItemTapped = new Command<OrderParameter>(OnItemSelected);
        }
        async void OnItemSelected(OrderParameter item)
        {
            if (item == null) return;
            try
            {
                if (item.OrderItems.Count() == 0) return;
            }
            catch (Exception)
            {
                return;
            }

            if(item.Status == "Placed" || item.Status == "Processing")
            {
                Device.BeginInvokeOnMainThread(async() =>
                {
                    if (isPressedAlready) return;
                    isPressedAlready = true;
                    var logoutPage = new YesOrNoPopupPage("Notice", "Ready to drive?", item);
                    logoutPage.OperationCompleted += Page_OperationCompleted;
                    await PopupNavigation.Instance.PushAsync(logoutPage);
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        isPressedAlready = false;
                        return false;
                    });
                });
                return;
            }
            await redirectToDetailPage(item);
        }

        private async void Page_OperationCompleted(object sender, EventArgs e)
        {
            var confirmationPage = (sender as YesOrNoPopupPage);
            //
            confirmationPage.OperationCompleted -= Page_OperationCompleted;
            await redirectToDetailPage(confirmationPage.order, "Placed");
        }

        async Task redirectToDetailPage(OrderParameter item, string status = null)
        {
            if (!string.IsNullOrEmpty(status))
            { 
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var ongoingOrderDetailPage = new OngoingOrderDetailPage();
                    ongoingOrderDetailPage.Order = item;
                    ongoingOrderDetailPage.OrderId = item.OrderId;
                    await Application.Current.MainPage.Navigation.PushAsync(ongoingOrderDetailPage);
                });
                return;
            }
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var ongoingOrderDetailPage = new OngoingOrderDetailPage();
                ongoingOrderDetailPage.Order = item;
                ongoingOrderDetailPage.OrderId = item.OrderId;
                await Application.Current.MainPage.Navigation.PushAsync(ongoingOrderDetailPage);
            });
        }
    }
}