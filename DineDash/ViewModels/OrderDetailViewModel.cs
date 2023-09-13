using DineDash.Enums;
using DineDash.Interface;
using DineDash.Models;
using DineDash.Utilities;
using DineDash.Views;
using DineDash.Views.Popup;
using DineDash.Views.Snackbar;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class OrderDetailViewModel : BaseViewModel
    {
        public Command AcceptOrderCommand { get; }
        public Command DeliveredOrderCommand { get; }
        public Command CancelOrderCommand { get; }
        public ObservableCollection<OrderItem> LineItems { get; }
        public Command UserInfoCommand { get; }
        public ObservableCollection<OpenOrderItem> OpenOrderLineItems { get; }
        ImageSource _MapImage = "satellite";
        public ImageSource MapImage
        {
            get => _MapImage;
            set
            {
                _MapImage = value;
                OnPropertyChanged("MapImage");
            }
        }
        private OrderParameter order;
        public OrderParameter Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged("Order");
                LoadOrder(value);
            }
        }
        private OpenOrderParameter openOrder;
        public OpenOrderParameter OpenOrder
        {
            get => openOrder;
            set
            {
                openOrder = value;
                OnPropertyChanged("Order");
                LoadOpenOrder(value);
            }
        }

        private string orderId;
        public string OrderId
        {
            get => orderId;
            set
            {
                orderId = value;
                OnPropertyChanged("OrderId");
            }
        }

        private OrderStatus status;
        public OrderStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private DateTime dateGmt;
        public DateTime DateGmt
        {
            get => dateGmt;
            set
            {
                dateGmt = value;
                OnPropertyChanged("DateGmt");
            }
        }


        private Address billingAddress;
        public Address BillingAddress
        {
            get => billingAddress;
            set
            {
                billingAddress = value;
                OnPropertyChanged("BillingAddress");
            }
        } 
        private Address shippingAddress;
        public Address ShippingAddress
        {
            get => shippingAddress;
            set
            {
                shippingAddress = value;
                OnPropertyChanged("ShippingAddress");
            }
        }

        private string changeAddress;
        public string ChangeAddress
        {
            get => changeAddress;
            set
            {
                changeAddress = value;
                OnPropertyChanged("ChangeAddress");
            }
        }
        private double additionalFee;
        public double AdditionalFee
        {
            get => additionalFee;
            set
            {
                additionalFee = value;
                OnPropertyChanged("AdditionalFee");
            }
        }

        private double total;
        public double Total
        {
            get => total;
            set
            {
                total = value;
                OnPropertyChanged("Total");
            }
        }

        private double shipping;
        public double Shipping
        {
            get => shipping;
            set
            {
                shipping = value;
                OnPropertyChanged("Shipping");
            }
        }
        private double discount;
        public double Discount
        {
            get => discount;
            set
            {
                discount = value;
                OnPropertyChanged("Discount");
            }
        }
        private double grandTotal;
        public double GrandTotal
        {
            get => grandTotal;
            set
            {
                grandTotal = value;
                OnPropertyChanged("GrandTotal");
            }
        }
        int id { get; set; }
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        
        bool isAcceptOrderView;
        public bool IsAcceptOrderView
        {
            get => isAcceptOrderView;
            set
            {
                isAcceptOrderView = value;
                OnPropertyChanged("IsAcceptOrderView");
            }
        }

        bool isNotAcceptOderView;
        public bool IsNotAcceptOderView
        {
            get => isNotAcceptOderView;
            set
            {
                isNotAcceptOderView = value;
                OnPropertyChanged("IsNotAcceptOderView");
            }
        }
        private double _gridHeight = DeviceDisplay.MainDisplayInfo.Height / 8;
        public double GridHeight
        {
            get => _gridHeight;
            set
            {
                if (_gridHeight != value)
                {
                    _gridHeight = value;
                    OnPropertyChanged(nameof(GridHeight));
                }
            }
        }
        private double initialY;
        public void OnPanStarted(double initialY)
        {
            this.initialY = initialY; // Store initial Y-coordinate
        }

        public void UpdateGridHeight(double newHeight)
        {
            GridHeight = newHeight;
        }
        public double CalculatePanDelta(double currentY)
        {
            // Calculate the change in Y-direction
            return currentY - initialY;
        }
        
        string eta;
        public string ETA
        {
            get => eta;
            set
            {
                eta = value;
                OnPropertyChanged("ETA");
            }
        }

        bool isNotAddressChange = true;
        public bool IsNotAddressChange
        {
            get => isNotAddressChange;
            set
            {
                isNotAddressChange = value;
                OnPropertyChanged("IsNotAddressChange");
            }
        }
        CustomerDetails customerDetails { get; set; }
        bool isClicked = false;
        void Communication()
        {
            MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrdersView");
            MessagingCenter.Subscribe<object>(this, "updateOngoingOrdersView", (args) =>
            {
                updateOrder();
            });
        }

        void updateOrder()
        {
            try
            {
                foreach (var onGoingOrder in Globals.OngoingOrders)
                {
                    if (Order?.OrderId == onGoingOrder.OrderId)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Order = onGoingOrder;
                        });
                        break;
                    }
                }
            }
            catch(Exception e)
            {

            }
        }

        public OrderDetailViewModel(bool _isAcceptOrderView)
        {
            this.IsAcceptOrderView = _isAcceptOrderView;
            if (IsAcceptOrderView)
            {
                IsNotAcceptOderView = false;
            }
            else
            {
                IsNotAcceptOderView = true;
            }
            LineItems = new ObservableCollection<OrderItem>();
            OpenOrderLineItems = new ObservableCollection<OpenOrderItem>();
            AcceptOrderCommand = new Command(async() =>
            {
                IsBusy = true;
                await Task.Delay(300);
                var res = await JsonWebApiAction.UpdateOrderSetDriverId(Globals.LoggedDriverId, OrderId, $"{Globals.StoreLon}", $"{Globals.StoreLat}");
                if(res == 1)
                {
                    var res1 = await App.RestaurantDatabase.DeleteOpenOrderAsyncById(Id);
                    var infopage = new InfoPopupPage("Info", "Successfully Accepted Order", "Okay");
                    infopage.OperationCompleted += InfoPageOperationCompleted;
                    await PopupNavigation.Instance.PushAsync(infopage);
                }
                if(res == 2)
                {
                    var res1 = await App.RestaurantDatabase.DeleteOpenOrderAsyncById(Id);
                    DisplayAlert("Order Id was already accepted by other driver!", Color.Orange);
                }

                if (res == 0)
                {
                    DisplayAlert("something went wrong!", Color.Orange);
                }
                IsBusy = false;
                //await Navigation.PushAsync(new AcceptOrderPage());
            });
            DeliveredOrderCommand = new Command(async () =>
            {
                UpdateAlert("Delivered");
            });
            CancelOrderCommand = new Command(async () =>
            {
                UpdateAlert("Cancelled");
            });
            UserInfoCommand = new Command(async() =>
            {
                if (isClicked) return;
                isClicked = true;
                IsBusy = true;
                await Task.Delay(300);

                bool isAvailableInternet = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                if (isAvailableInternet)
                {
                    customerDetails = await JsonWebApiAction.CheckUserInfoByCustomerId(Order.CustomerId);
                }
                if (customerDetails?.FullName == null)
                {
                    IsBusy = false;
                    return;
                }
                var customerDet = customerDetails;
                customerDet.Phone = !string.IsNullOrEmpty(customerDet.Phone) ? "+" + customerDet?.CountryCode + customerDet?.Phone : "";
                customerDet.Address = Order.Address;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await PopupNavigation.Instance.PushAsync(new CustomerInfoPopupPage(customerDet));
                });
                IsBusy = false;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    isClicked = false;
                    return false;
                });
            });
            Communication();
        }
        async void UpdateAlert(string status)
        {
            IsBusy = true;
            await Task.Delay(300);
            //if (status=="Delivered")
            //{
            //    if (!Globals.IsOnTheWayInitialize)
            //    {
            //        DisplayAlert("initialize on the way status first");
            //        return;
            //    }
            //}
            var result = await CurrentPage.DisplayAlert("Alert!", $"Do you really want to set order status to {status}?", "Yes", "No");

            if (result)
            {
                bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                if (!isAvailable)
                {
                    DisplayAlert("Turn on your internet connection", Color.Orange);
                    return;
                }
                var res = await JsonWebApiAction.UpdateOrderStatus(OrderId, status);
                if(res == 0)
                {
                    DisplayAlert("something went wrong.");
                }
                else
                {
                    var ongoing = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
                    //var ongoing = await service.GetOrderByOngoingStatus(Globals.LoggedCustomerId);
                    if (ongoing?.Count() != 0 && ongoing != null)
                    {
                        Globals.OngoingOrders = new ObservableCollection<OrderParameter>(ongoing);
                    }
                    else
                    {
                        Globals.OngoingOrders = null;
                    }
                    MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
                    MessagingCenter.Send<object>(this, "updateOngoingOrders2");
                    DependencyService.Get<Toast>().Show($"successfully updated order status");
                    await Navigation.PopAsync();
                    //MessagingCenter.Unsubscribe<object>(this, "FetchAndSendRecordsByDriverId");
                    //MessagingCenter.Send<object>(this, "FetchAndSendRecordsByDriverId");
                }
                IsBusy = false;
            }
            else
            {
                IsBusy = false;
            }
        }

        void DisplayAlert(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await PopupNavigation.Instance.PushAsync(new InfoPopupPage("Info", message, "Okay"));
            });
        }
        void DisplayAlert(string message, Color BG)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ToastSnackbar.DisplaySnackbar(CurrentPage, $"{message}", 2, BG);
            });
        }

        private void InfoPageOperationCompleted(object sender, EventArgs e)
        {
            var confirmationPage = (sender as InfoPopupPage);
            confirmationPage.OperationCompleted -= InfoPageOperationCompleted;
            Navigation.PopAsync();
        }

        public async void LoadOrder(OrderParameter ords)
        {
            //var order = await service.GetOrderAsync(id);
            //var lineItems = await service.GetOrderItemsAsync(id);

            if (ords.Status == "Placed")
                Status = OrderStatus.Placed;
            if (ords.Status == "Processing")
                Status = OrderStatus.Processing;
            if (ords.Status == "OnTheWay")
                Status = OrderStatus.OnTheWay;
            if (ords.Status == "Delivered")
                Status = OrderStatus.Delivered;
            if (ords.Status == "Cancelled")
                Status = OrderStatus.Cancelled;
            Device.BeginInvokeOnMainThread(() =>
            {

                Id = ords.Id;
                OrderId = ords.OrderId;
                DateGmt = ords.DateGmt;
                Total = ords.Total;
                GrandTotal = ords.GrandTotal;
                var addrss = new Address();
                addrss.Title = ords.IsChangeAddress ? ords.ChangeAddressTitle : ords.AddressTitle;
                addrss.Address1 = ords.IsChangeAddress ? ords.ChangeAddress : ords.Address;
                BillingAddress = addrss;//order.BillingAddress;
                ShippingAddress = addrss;//order.ShippingAddress;
                Shipping = ords.Shipping;
                Discount = ords.Discount;
                ChangeAddress = ords?.ChangeAddress;
                var IsChangeAddressAvailable = !ords.IsChangeAddress;
                if (!IsChangeAddressAvailable)
                {
                    IsNotAddressChange = false;
                }
                AdditionalFee = ords.AdditionalFee;
                try
                {
                    LineItems.Clear();
                }
                catch (Exception e)
                {

                }

                //foreach (var item in ords.OrderItems.ToList())
                //{
                for (int i = 0; i < ords.OrderItems.Count; i++)
                {
                    var item = ords.OrderItems[i];
                    item.Total = Math.Round(item.UnitPrice * item.Quantity, 2);
                    LineItems.Add(item);
                }
            });
        }
        public async void LoadOpenOrder(OpenOrderParameter ords)
        {
            //var order = await service.GetOrderAsync(id);
            //var lineItems = await service.GetOrderItemsAsync(id);

            if (ords.Status == "Placed")
                Status = OrderStatus.Placed;
            if (ords.Status == "Processing")
                Status = OrderStatus.Processing;
            if (ords.Status == "OnTheWay")
                Status = OrderStatus.OnTheWay;
            if (ords.Status == "Delivered")
                Status = OrderStatus.Delivered;
            if (ords.Status == "Cancelled")
                Status = OrderStatus.Cancelled;
            Device.BeginInvokeOnMainThread(() =>
            {

                Id = ords.Id;
                OrderId = ords.OrderId;
                DateGmt = ords.DateGmt;
                Total = ords.Total;
                GrandTotal = ords.GrandTotal;
                var addrss = new Address();
                addrss.Title = ords.AddressTitle;
                addrss.Address1 = ords.Address;
                BillingAddress = addrss;//order.BillingAddress;
                ShippingAddress = addrss;//order.ShippingAddress;
                Shipping = ords.Shipping;
                Discount = ords.Discount;
                ChangeAddress = ords?.ChangeAddress;
                AdditionalFee = ords.AdditionalFee;
                try
                {
                    OpenOrderLineItems.Clear();
                }
                catch (Exception e)
                {

                }

                //foreach (var item in ords.OrderItems.ToList())
                //{
                for (int i = 0; i < ords.OrderItems.Count; i++)
                {
                    var item = ords.OrderItems[i];
                    item.Total = Math.Round(item.UnitPrice * item.Quantity, 2);
                    OpenOrderLineItems.Add(item);
                }
            });
        }
    }
}
