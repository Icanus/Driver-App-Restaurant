using DineDash.Enums;
using DineDash.Helpers;
using DineDash.Interface;
using DineDash.Models;
using DineDash.Services;
using DineDash.Utilities;
using DineDash.Views;
using DineDash.Views.Popup;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Search.Common;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public Command LoadItemsCommand { get; }
        public Command OpenMenu { get; }
        public Command<OpenOrderParameter> ItemTapped { get; }
        public Command<OpenOrderParameter> ExpandCommand { get; }
        public Command ViewOrderCommand { get; }
        public ObservableCollection<OpenOrderParameter> Items { get; }
        bool isInitializing = false;
        public bool IsInitializing
        {
            get => isInitializing;
            set
            {
                isInitializing = value;
                OnPropertyChanged("IsInitializing");
            }
        }
        bool isPressedAlready = false;
        bool hasOrders = false;
        public bool HasOrders
        {
            get => hasOrders;
            set
            {
                hasOrders = value;
                OnPropertyChanged("HasOrders");
            }
        }
        string orderCount;
        public string OrderCount
        {
            get => orderCount;
            set
            {
                orderCount = value;
                OnPropertyChanged("OrderCount");
            }
        }

        string viewOrderText;
        public string ViewOrderText
        {
            get => viewOrderText;
            set
            {
                viewOrderText = value;
                OnPropertyChanged("ViewOrderText");
            }
        }
        
        Thickness refreshMargin = new Thickness(0, 0, 0, 0);
        public Thickness RefreshMargin
        {
            get => refreshMargin;
            set
            {
                refreshMargin = value;
                OnPropertyChanged("RefreshMargin");
            }
        }
        private static bool isTimerRunning = false;

        ObservableCollection<OrderParameter> ongoingOrder;
        public ObservableCollection<OrderParameter> OngoingOrders
        {
            get => ongoingOrder;
            set
            {
                ongoingOrder = value;
                OnPropertyChanged("OngoingOrders");
            }
        }
        string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        ImageSource imageFile = "upload";
        public ImageSource ImageFile
        {
            get => imageFile;
            set
            {
                imageFile = value;
                OnPropertyChanged("ImageFile");
            }
        }
        public HomeViewModel()
        {
            Items = new ObservableCollection<OpenOrderParameter>();
            LoadItemsCommand = new Command(async() => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<OpenOrderParameter>(OnItemSelected);
            ExpandCommand = new Command<OpenOrderParameter>(async (args) =>
            {
                args.IsExpanded = !args.IsExpanded;
            });
            OpenMenu = new Command(async () =>
            {
                if (isPressedAlready) return;
                isPressedAlready = true;

                //etooo
                //Globals.isFromItemsPage = false;
                MessagingCenter.Unsubscribe<object>(this, "UpdateLoginStatus");
                MessagingCenter.Send<object>(this, "UpdateLoginStatus");

                await Navigation.PushPopupAsync(new MenuPopupPage());

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    isPressedAlready = false;
                    return false;
                });
            });
            ViewOrderCommand = new Command(async () =>
            {
                var locationSettingsService = DependencyService.Get<ILocationSettingsService>();

                if (!locationSettingsService.IsGpsTurnedOn())
                {
                    await locationSettingsService.OpenLocationSettingsAsync();
                    return;
                }
                await Navigation.PushAsync(new OngoingOrdersPage(OngoingOrders));
            });
            Communication();
        }
        void updateOngoingOrders(ObservableCollection<OrderParameter> args2)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (args2.Count != 0)
                {
                    HasOrders = true;
                    OrderCount = args2.Count.ToString();
                    OngoingOrders = args2;
                    ViewOrderText = $"View Orders: {OrderCount}";
                }
                else
                {
                    HasOrders = false;
                }
            });
        }
        void Communication()
        {
            MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders");
            MessagingCenter.Subscribe<object, ObservableCollection<OrderParameter>>(this, "updateOngoingOrders", async (args, args2) =>
            {
                updateOngoingOrders(args2);
            });

            MessagingCenter.Unsubscribe<object>(this, "LogoutCommand");
            MessagingCenter.Subscribe<object>(this, "LogoutCommand", (args) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var logoutPage = new LogoutPopupPage();
                    logoutPage.OperationCompleted += LogoutPage_OperationCompleted;
                    await PopupNavigation.Instance.PushAsync(logoutPage);
                });
            });

            MessagingCenter.Unsubscribe<object>(this, "GetOpenOrders");
            MessagingCenter.Subscribe<object, List<OpenOrderParameter>>(this, "GetOpenOrders", (args,args2) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    GetOpenOrders(args2);
                });
            });

            MessagingCenter.Unsubscribe<object>(this, "OnSleepLogout");
            MessagingCenter.Subscribe<object>(this, "OnSleepLogout", (sender) => {
                logoutFunctionV2();
            });
        }

        public void DisconnectSubs()
        {
            MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders");
            MessagingCenter.Unsubscribe<object>(this, "LogoutCommand");
            MessagingCenter.Unsubscribe<object>(this, "GetOpenOrders");
            MessagingCenter.Unsubscribe<object>(this, "OnSleepLogout");
        }

        private async void LogoutPage_OperationCompleted(object sender, EventArgs e)
        {
            logoutFunction();
        }
        async void logoutFunction()
        {
            Globals.IsInitialized = false;
            Globals.IsLogin = false;
            Globals.LoggedDriverId = null;
            //Globals.IsDriving = false;
            isTimerRunning = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                IGoogleManager _googleManager = DependencyService.Get<IGoogleManager>();
                if (Globals.IsLoginByGoogle)
                {
                    Globals.IsLoginByGoogle = false;
                    _googleManager.Logout();
                }
            }

            MessagingCenter.Unsubscribe<object>(this, "CloseMenu");
            MessagingCenter.Send<object>(this, "CloseMenu");
            DisconnectSubs();

            await Navigation.PopAsync();
        }
        async void logoutFunctionV2()
        {
            Globals.IsInitialized = false;
            Globals.IsLogin = false;
            Globals.LoggedDriverId = null;
            //Globals.IsDriving = false;
            isTimerRunning = false;
            if (Device.RuntimePlatform == Device.Android)
            {
                IGoogleManager _googleManager = DependencyService.Get<IGoogleManager>();
                if (Globals.IsLoginByGoogle)
                {
                    Globals.IsLoginByGoogle = false;
                    _googleManager.Logout();
                }
            }
            DisconnectSubs();
            try
            {
                await Navigation.PopAsync();
            }
            catch(Exception e)
            {

            }
        }
        public void OnAppearing()
        {
            IsBusy = true;
        }
        bool isTaskComplete = false;
        async Task ExecuteLoadItemsCommand()
        {
            Globals.lastSyncId = 0;
            IsBusy = true;
            isTaskComplete = false;
            try
            {
                Items.Clear();
                //var items = await service.GetOrdersAsync(Globals.LoggedCustomerId);
                //foreach (var item in items)
                //{
                //    Items.Add(item);
                //}
                //await JsonWebApiAction.Get
                bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                var customer = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);

                var vehicle = await App.RestaurantDatabase.GetVehicleAsync(Globals.LoggedDriverId);
                if (string.IsNullOrEmpty(vehicle.DriverId))
                {
                    if (isAvailable)
                    {
                        vehicle = await JsonWebApiAction.GetVehicleByDriverId(Globals.LoggedDriverId);
                        if (!string.IsNullOrEmpty(vehicle.DriverId))
                        {
                            await App.RestaurantDatabase.UpsertVehicle(vehicle);
                        }
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    FullName = customer?.FullName;
                    ImageFile = vehicle?.DriverId != null ? vehicle.DriversPhoto : "no_camera";
                    if (Globals.IsLoginByGoogle)
                    {
                        try
                        {
                            if (isAvailable)
                            {
                                byte[] bytes;
                                using (WebClient client = new WebClient())
                                {
                                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                                    bytes = client.DownloadData(customer.Image);
                                }
                                Stream stream = new MemoryStream(bytes);
                                ImageFile = ImageSource.FromStream(() => { return stream; });
                            }
                        }
                        catch(Exception e)
                        {

                        }
                    }
                });
                List<OpenOrderParameter> parameters = new List<OpenOrderParameter>();
                List<OpenOrder> openOrders = new List<OpenOrder>();
                List<OpenOrderItem> openOrderItems = new List<OpenOrderItem>();
                List<OrderParameter> ongoingOrders1 = new List<OrderParameter>();
                if (isAvailable)
                {
                    parameters = await JsonWebApiAction.GetOpenOrders(Globals.lastSyncId);
                    ongoingOrders1 = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
                    GetOpenOrders(parameters);
                    if (ongoingOrders1 != null && ongoingOrders1?.Count() != 0)
                    {

                        checkIfDriving(ongoingOrders1);

                        var ongoing = new ObservableCollection<OrderParameter>(ongoingOrders1);
                        Globals.OngoingOrders = ongoing;
                        OngoingOrders = ongoing;
                        updateOngoingOrders(ongoing);
                        MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
                        MessagingCenter.Send<object>(this, "updateOngoingOrders2");

                        MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrdersView");
                        MessagingCenter.Send<object>(this, "updateOngoingOrdersView");
                    }

                    if (!isTimerRunning)
                    {
                        isTimerRunning = true;
                        Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                        {
                            if (isTaskComplete)
                            {
                                //Globals.IsDriving = false;
                                isTimerRunning = false;
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    HasOrders = false;
                                    RefreshMargin = new Thickness(0);
                                });
                                return false;
                            }
                            GetOngoingOrdersAsync();
                            // Perform your asynchronous task here


                            return true;
                        });
                    }

                }

            }
            catch (Exception e)
            {

            }
            IsBusy = false;
        }

        void checkIfDriving(List<OrderParameter> ongoing)
        {
            //try
            //{
            //    var isDriving = ongoing.Where(x => x.Status == "OnTheWay").ToList();
            //    Globals.IsDriving = isDriving.Count() > 0 ? true : false;
            //}
            //catch(Exception e)
            //{
            //    Globals.IsDriving = false;
            //}
        }

        async void GetOngoingOrdersAsync()
        {
            int counter = 0;
            var ongoing = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
            //var ongoing = await service.GetOrderByOngoingStatus(Globals.LoggedCustomerId);
            if (ongoing?.Count() != 0 && ongoing != null)
            {
                checkIfDriving(ongoing);
                HasOrders = true;
                var ongoingOrd = new ObservableCollection<OrderParameter>(ongoing);
                OngoingOrders = ongoingOrd;
                Globals.OngoingOrders = OngoingOrders;
                RefreshMargin = new Thickness(0, 0, 0, 100);
                //Globals.OngoingOrder = OngoingOrders;
            }
            if (HasOrders)
            {
                try
                {
                    var ongoingOrd = new ObservableCollection<OrderParameter>(ongoing);
                    OngoingOrders = ongoingOrd;
                    var hasValue = true;
                    hasValue = await GetOngoingOrdersBoolean();
                }catch(Exception e)
                {
                    //Globals.IsDriving = false;
                    HasOrders = false;
                    isTimerRunning = false;
                }
            }
            else
            {
                //Globals.IsDriving = false;
                isTimerRunning = false;
                isTaskComplete = true;
            }
        }
        async Task<bool> GetOngoingOrdersBoolean()
        {
            var ongoing1 = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
            if (ongoing1?.Count() != 0 && ongoing1 != null)
            {
                var ongoingOrd = new ObservableCollection<OrderParameter>(ongoing1);
                checkIfDriving(ongoing1);
                HasOrders = true;
                OngoingOrders = ongoingOrd;
                Globals.OngoingOrders = OngoingOrders;
                MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
                MessagingCenter.Send<object>(this, "updateOngoingOrders2");

                MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrdersView");
                MessagingCenter.Send<object>(this, "updateOngoingOrdersView");

                return true;
            }
            else
            {
                //Globals.IsDriving = false;
                isTaskComplete = true;
                HasOrders = false;
                Globals.OngoingOrders = null;
                RefreshMargin = new Thickness(0);
                return false;
            }
        }

        async void GetOpenOrders(List<OpenOrderParameter> res)
        {
            List<int> closedOrders = new List<int>();
            List<OpenOrder> orders = new List<OpenOrder>();
            List<OpenOrderItem> orderItems = new List<OpenOrderItem>();
            if (res == null)
            {
                IsBusy = false;
                return;
            }
            foreach (var item in res)
            {
                OpenOrder order = new OpenOrder
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.CustomerId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Discount = item.Discount,
                    Total = item.Total,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    Status = item.Status,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    ForPickUpTime = item.ForPickUpTime,
                    OnTheWayTime = item.OnTheWayTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                };

                foreach (var orditem in item.OrderItems)
                {
                    OpenOrderItem itm = new OpenOrderItem
                    {
                        Id = orditem.Id,
                        OrderId = orditem.OrderId,
                        ProductId = orditem.ProductId,
                        ProductName = orditem.ProductName,
                        ProductImage = orditem.ProductImage,
                        ProductDescription = orditem.ProductDescription,
                        UnitPrice = orditem.UnitPrice,
                        Quantity = orditem.Quantity,
                        IngredientString = orditem.IngredientString,
                        ChoiceString = orditem.ChoiceString,
                        Total = orditem.Total,
                    };
                    orderItems.Add(itm);
                }
                orders.Add(order);
            }

            //var closedOrdersResult = await JsonWebApiAction.GetClosedOrders(Globals.lastSyncId);
            //if (closedOrdersResult != null)
            //{
              //  foreach (var item in closedOrdersResult)
                //{
                  //  closedOrders.Add(item.Id);
                //}
                await App.RestaurantDatabase.DeleteOpenOrderNotInOrderList(orders);
            //}

            var res1 = await App.RestaurantDatabase.InsertOpenOrder(orders);
            var res2 = await App.RestaurantDatabase.AddAllOpenOrderItem(orderItems);
            var ordersss = await App.RestaurantDatabase.GetOpenOrders();
            Console.WriteLine(ordersss);
            foreach (var value in ordersss)
            {
                if (value.Status == "Placed")
                    value.OrderStatus = OrderStatus.Placed;
                if (value.Status == "Processing")
                    value.OrderStatus = OrderStatus.Processing;
                if (value.Status == "OnTheWay")
                    value.OrderStatus = OrderStatus.OnTheWay;
                if (value.Status == "Delivered")
                    value.OrderStatus = OrderStatus.Delivered;
                if (value.Status == "Cancelled")
                    value.OrderStatus = OrderStatus.Cancelled;
                value.Address2Display = value.Address;
                if (value.IsChangeAddress)
                    value.Address2Display = value.ChangeAddress;
                Items.Add(value);
            }

            var getHasNewOrderItems = await App.RestaurantDatabase.GetHasNewOpenOrders(Globals.LoggedDriverId);
                if(getHasNewOrderItems.HasNewItems)
                    DependencyService.Get<ISoundPlayer>().PlaySound("ping.mp3");
        }

        async void OnItemSelected(OpenOrderParameter item)
        {
            //if(Globals.IsDriving)
            //{
            //    await PopupNavigation.Instance.PushAsync(new InfoPopupPage("Requested service not available", "The Delivery service you are trying to access is unavailable.", "Okay"));
            //    return;
            //}
            if (item == null) return;

            var orderDetailPage = new OpenOrderDetailPage();
            orderDetailPage.Order = item;
            try
            {
                var res = await App.RestaurantDatabase.UpdateIsViewedOpenOrders(item.Id);
            }
            catch (Exception)
            {

            }
            await Navigation.PushAsync(orderDetailPage);
        }
    }
}
