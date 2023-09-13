using DineDash.Models;
using DineDash.Utilities;
using DineDash.Views.Popup;
using DineDash.Views.Snackbar;
using GoogleApi.Entities.Search.Video.Common.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DineDash.ViewModels
{
    internal class OngoingOrderViewModel : BaseViewModel
    {
        bool IsAddressChangePrompt = false;
        int IsAddressChangeCount = 0;
        List<OrderParameter> AddressChangeOrderIdList;
        public Command InitiateOnTheWayCommand { get; }
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

        bool _OnTheWayVisibility = false;
        public bool OnTheWayVisibility
        {
            get => _OnTheWayVisibility;
            set
            {
                _OnTheWayVisibility = value;
                OnPropertyChanged("OnTheWayVisibility");
            }
        }
        bool _HasOrders = false;
        public bool HasOrders
        {
            get => _HasOrders;
            set
            {
                _HasOrders = value;
                OnPropertyChanged("HasOrders");
            }
        }
        string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
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
        ObservableCollection<OrderParameter> clonedCollection;
        public OngoingOrderViewModel(ObservableCollection<OrderParameter> param)
        {
            clonedCollection = new ObservableCollection<OrderParameter>();
            OngoingOrders = param;
            if(OngoingOrders.Count() > 0)
            {
                HasOrders = true;
                clonedCollection = new ObservableCollection<OrderParameter>(param);
                InitializeValues(clonedCollection);
            }
            InitiateOnTheWayCommand = new Command(async() =>
            {
                IsBusy = true;
                await Task.Delay(300);
                IsAddressChangeCount = 0;
                AddressChangeOrderIdList = new List<OrderParameter>();
                var col = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
                clonedCollection = new ObservableCollection<OrderParameter>();
                clonedCollection = new ObservableCollection<OrderParameter>(col);
                //foreach (var order in clonedCollection.ToList())
                //{
                for (int i = 0; i < clonedCollection.Count; i++)
                {
                    var order = clonedCollection[i];

                    if (clonedCollection[i].IsChangeAddress && !clonedCollection[i].IsChangeAddressAccepted)
                    {
                        if (!clonedCollection[i].IsChangeAddressAccepted)
                        {
                            IsAddressChangeCount++;
                            AddressChangeOrderIdList.Add(clonedCollection[i]);
                        }
                    }
                }
                if(IsAddressChangeCount > 0)
                {
                    IsAddressChangePrompt = true;
                    if (IsAddressChangePrompt && IsAddressChangeCount > 0)
                    {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var infopage = new InfoPopupPage("Info", $"a customer(s) has requested to change it's delivery address", "Accept Changes");
                                infopage.OperationCompleted -= InfoPageOperationCompleted;
                                infopage.OperationCompleted += InfoPageOperationCompleted;
                                await PopupNavigation.Instance.PushAsync(infopage);
                                IsAddressChangeCount = 0;
                                OnTheWayVisibility = true;
                            });
                    }
                    IsBusy = false;
                    return;
                }
                else
                {
                    var result = await CurrentPage.DisplayAlert("Notice!", "Ready to drive?", "Yes", "No");

                    if (result)
                    {
                        for (int i = 0; i < clonedCollection.Count; i++)
                        {
                            var order = clonedCollection[i];
                            await JsonWebApiAction.UpdateOrderStatus(order.OrderId, "OnTheWay");
                            order.Status = "OnTheWay";
                        }

                        for (int i = 0; i < OngoingOrders.Count; i++)
                        {
                            OngoingOrders[i].Status = "OnTheWay";
                        }
                        //Globals.IsOnTheWayInitialize = false;
                        OnTheWayVisibility = false;
                        IsBusy = false;
                        //Globals.IsDriving = true;
                        DisplayAlert("Successfully change order status", Color.Green);

                    }
                    else
                    {
                        IsBusy = false;
                    }
                }
            });
        }
        async void InitializeValues(ObservableCollection<OrderParameter> clonedCollection)
        {
            try
            {
                var filteredCollection = new ObservableCollection<OrderParameter>(clonedCollection.Where(delivery => delivery.OnTheWayTime.HasValue));
                if (filteredCollection.Count > 0)
                {
                    for (int i = 0; i < clonedCollection.Count; i++)
                    {
                        var order = clonedCollection[i];
                        await JsonWebApiAction.UpdateOrderStatus(order.OrderId, "OnTheWay");
                        order.Status = "OnTheWay";
                    }
                    //Globals.IsOnTheWayInitialize = true;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OnTheWayVisibility = false;
                    });
                }
                else
                {
                    //Globals.IsOnTheWayInitialize = false;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OnTheWayVisibility = true;
                    });
                }
            }
            catch (Exception e)
            {
                //Globals.IsOnTheWayInitialize = false;
                Device.BeginInvokeOnMainThread(() =>
                {
                    OnTheWayVisibility = true;
                });
            }
        }
        private async void InfoPageOperationCompleted(object sender, EventArgs e)
        {
            var confirmationPage = (sender as InfoPopupPage);
            confirmationPage.OperationCompleted -= InfoPageOperationCompleted;
            foreach (var order in AddressChangeOrderIdList.ToList())
            {
                await JsonWebApiAction.AcceptAddressChange(order.OrderId, order.DriverId);
            }
            var ongoing = await JsonWebApiAction.GetOngoingOrdersByDriverId(Globals.LoggedDriverId);
            //var ongoing = await service.GetOrderByOngoingStatus(Globals.LoggedCustomerId);
            if (ongoing?.Count() != 0 && ongoing != null)
            {
                Globals.OngoingOrders = new ObservableCollection<OrderParameter>(ongoing);
            }
            MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
            MessagingCenter.Send<object>(this, "updateOngoingOrders2");
            DisplayAlert("Successfully accepted change(s)", Color.Green);
            IsAddressChangePrompt = false;
            IsAddressChangeCount = 0;
        }
        public void DisplayAlert(string message, Color BG)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ToastSnackbar.DisplaySnackbar(CurrentPage, $"{message}", 2, BG);
            });
        }
    }
}
