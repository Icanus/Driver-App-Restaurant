using DineDash.Helpers;
using DineDash.Interface;
using DineDash.Models;
using DineDash.Utilities;
using DineDash.ViewModels;
using DineDash.Views.Popup;
using DineDash.Views.Snackbar;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace DineDash.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OngoingOrdersPage : ContentPage
    {
        private bool isAnimating = false;
        private Rectangle currentLayoutBounds;
        CustomMap customMap;
        OngoingOrderViewModel viewModel;
        public ObservableCollection<OrderParameter> Order { get; set; }
        private bool isTaskComplete = false;
        public string Address { get; set; }
        private static bool isTimerRunning = false;
        Models.Vehicle vehicle { get; set; }
        Models.Driver driver { get; set; }

        public OngoingOrdersPage(ObservableCollection<OrderParameter> param)
        {
            InitializeComponent();
            Order = param;
            viewModel = new OngoingOrderViewModel(param);
            BindingContext = viewModel;
            Communication();
            PrepareMap();
            currentLayoutBounds = new Rectangle(0.5, 1, 1, 0.5);
            UpdateStackLayoutLayoutBounds();
        }


        private async void OnSwipeUp(object sender, SwipedEventArgs e)
        {
            await AnimateLayoutChange(new Rectangle(0.5, 1, 1, 0.5));
        }

        private async void OnSwipeDown(object sender, SwipedEventArgs e)
        {
            await AnimateLayoutChange(new Rectangle(0.5, 1, 1, 0.035));
        }

        private async Task AnimateLayoutChange(Rectangle newLayoutBounds)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var animationDuration = 250; // Adjust the animation duration as needed
            var easing = Easing.CubicInOut; // Choose an easing function

            var startBounds = currentLayoutBounds; // Store the starting layout bounds
            var animation = new Animation(v =>
            {
                currentLayoutBounds = new Rectangle(
                    startBounds.X + (newLayoutBounds.X - startBounds.X) * v,
                    startBounds.Y + (newLayoutBounds.Y - startBounds.Y) * v,
                    startBounds.Width + (newLayoutBounds.Width - startBounds.Width) * v,
                    startBounds.Height + (newLayoutBounds.Height - startBounds.Height) * v);

                UpdateStackLayoutLayoutBounds();
            });

            animation.Commit(this, "LayoutAnimation", length: (uint)animationDuration, easing: easing);

            await Task.Delay(animationDuration);
            isAnimating = false;
        }

        private void UpdateStackLayoutLayoutBounds()
        {
            AbsoluteLayout.SetLayoutBounds(stackLayout, currentLayoutBounds);
        }
        void Communication()
        {
            //MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
            //MessagingCenter.Subscribe<object>(this, "updateOngoingOrders2", (args) =>
            //{
            //    updateOrder();
            //});
        }

        async void updateOrder()
        {
            try
            {
                Order = Globals.OngoingOrders;
                viewModel.OngoingOrders = Globals.OngoingOrders;
            }
            catch(Exception e)
            {

            }
            await UpdateLocation();
        }

        private async void PrepareMap()
        {
            try
            {
                vehicle = await App.RestaurantDatabase.GetVehicleAsync(Globals.LoggedDriverId);
                driver = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);

                viewModel.IsBusy = true;
                await Task.Delay(300);
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request).ConfigureAwait(true);
                if (location != null)
                {
                    Globals.lastlatitude = location.Latitude; //double.Parse("8.13463");
                    Globals.lastlongitude = location.Longitude; //double.Parse("-13.30254");
                }
                customMap = new CustomMap
                {
                    MapType = MapType.Street,
                    IsShowingUser = false,
                    WidthRequest = 150,
                    HeightRequest = 150,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(Globals.lastlatitude, Globals.lastlongitude), Xamarin.Forms.Maps.Distance.FromKilometers(10));

                Device.BeginInvokeOnMainThread(() =>
                {
                    // Set the default map region
                    customMap.MoveToRegion(mapSpan);
                    customMap.MapType = MapType.Street;
                    customMap.TrafficEnabled = true;
                });


                if (mystacklayout.Children.Any())
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        mystacklayout.Children.Clear();
                    });
                }
                if (customMap.Pins.Count() > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        customMap.Pins.Clear();
                    });
                }
                List<CustomPin> customPinsList = new List<CustomPin>();
                CustomPin driverPin = new CustomPin
                {
                    Name = $"Driver",
                    VehicleName = vehicle != null ? vehicle.CarDescription : "",
                    VehiclePhoto = vehicle != null ? vehicle.CarPhoto : "no_camera",
                    DriverName = $"{driver.FullName}",
                    DriverPhoto = vehicle != null ? vehicle.DriversPhoto : "no_camera",
                    Url = "",
                    Type = PinType.Place,
                    Position = new Position(Globals.lastlatitude, Globals.lastlongitude),
                    Label = $"Driver's Location",
                    Address = $"Coordinate's: {Globals.lastlatitude}, {Globals.lastlongitude}"
                }; 
                Device.BeginInvokeOnMainThread(() =>
                {
                    customMap.Pins.Clear();
                    customMap.Pins.Add(driverPin);
                    customPinsList.Add(driverPin);
                });
                foreach (var position in viewModel.OngoingOrders.ToList())
                {
                    CustomPin devicePin = new CustomPin
                    {
                        Name = $"{position.Id}",
                        Url = "",
                        Type = PinType.Place,
                        Position = new Position(Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLat : position.Lat), Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLon : position.Lon)),
                        Label = $"Order: ORD-000{position.Id}",
                        Address = $"Title: {position.AddressTitle}, Address: {position.Address}"
                    };
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        customMap.Pins.Add(devicePin);
                    });
                    customPinsList.Add(devicePin);
                    break;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    customMap.CustomPins = customPinsList;
                    mystacklayout.Children.Add(customMap);
                });
                
                await UpdateLocation();
                #region hide
                //if (!isTimerRunning)
                //{
                //    isTimerRunning = true;
                //    Device.StartTimer(TimeSpan.FromSeconds(15), () =>
                //    {
                //        if (isTaskComplete)
                //        {
                //            isTimerRunning = false;
                //            return false;
                //        }

                //        // Perform your asynchronous task here
                //        _ = UpdateLocation();
                //        if (IsAddressChangePrompt && IsAddressChangeCount>0)
                //        {
                //            if (IsAddressChangePrompt)
                //            {
                //                IsAddressChangeCount = 0;
                //                return true;
                //            }
                //            IsAddressChangePrompt = false;
                //            Device.BeginInvokeOnMainThread(async () =>
                //            {
                //                var infopage = new InfoPopupPage("Info", $"{IsAddressChangeCount} customer has requested to change it's delivery address", "Accept Changes");
                //                infopage.OperationCompleted -= InfoPageOperationCompleted;
                //                infopage.OperationCompleted += InfoPageOperationCompleted;
                //                await PopupNavigation.Instance.PushAsync(infopage);
                //                IsAddressChangeCount = 0;
                //            });
                //            return false;
                //        }
                //        IsAddressChangeCount = 0;
                //        // Return true to continue the timer
                //        return true;
                //    });
                //}
                //customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lat, lon), Distance.FromMiles(0.50f)));
                #endregion
            }
            catch (Exception ex)
            {
                viewModel.IsBusy = false;
                //await DisplayAlert("Error", ex.Message, "OK").ConfigureAwait(true);
            }
        }

        async Task UpdateLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request).ConfigureAwait(true);
                if (location != null)
                {
                    Globals.lastlatitude = location.Latitude; //double.Parse("8.13463");
                    Globals.lastlongitude = location.Longitude; //double.Parse("-13.30254");
                }
                if(viewModel.OngoingOrders == null)
                {
                    isTaskComplete = true;
                    if (customMap.Pins.Count() > 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            viewModel.HasOrders = false;
                            customMap.Pins.Clear();
                            customMap.CustomPins.Clear();
                        });


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            customMap.MapElements.Clear();
                            viewModel.IsBusy = false;
                        });
                    }
                    return;
                }
                if (viewModel.OngoingOrders.Count() == 0)
                {
                    isTaskComplete = true;
                    if (customMap.Pins.Count() > 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            customMap.Pins.Clear();
                            customMap.CustomPins.Clear();
                        });


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            customMap.MapElements.Clear();
                            viewModel.IsBusy = false;
                        });
                    }
                    return;
                }
                foreach (var order in viewModel.OngoingOrders.ToList())
                {
                    order.DriverLat = Globals.lastlatitude.ToString();
                    order.DriverLon = Globals.lastlongitude.ToString();
                    await JsonWebApiAction.UpdateOrderLocationByDriverId(order.DriverId, order.OrderId, Globals.lastlongitude.ToString(), Globals.lastlatitude.ToString());
                    break;
                }
                int count = 0;
                var lat = 0.00;
                var lon = 0.00;
                List<Position> positions = new List<Position>();
                List<Position> pos = new List<Position>();
                foreach (var ord in viewModel.OngoingOrders.ToList())
                {
                    //double.Parse("-13.30254");
                    count++;
                    var latitude = Convert.ToDecimal(ord.IsChangeAddressAccepted ? ord.ChangeAddressLat : ord.Lat);
                    var longitude = Convert.ToDecimal(ord.IsChangeAddressAccepted ? ord.ChangeAddressLon : ord.Lon);
                    var position = new Position((double)latitude, (double)longitude);
                    positions.Add(position);
                    if (count == 1)
                    {

                        pos.Add(new Position(Convert.ToDouble(ord.DriverLat), Convert.ToDouble(ord.DriverLon)));
                        lat = (double)latitude;
                        lon = (double)longitude;
                        viewModel.Id = ord.Id.ToString();
                        break;
                    }
                }
                if (customMap.Pins.Count() > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        customMap.Pins.Clear();
                    });
                }
                List<CustomPin> customPinsList = new List<CustomPin>();
                CustomPin driverPin = new CustomPin
                {
                    Name = $"Driver",
                    VehicleName = vehicle != null ? vehicle.CarDescription : "",
                    VehiclePhoto = vehicle != null ? vehicle.CarPhoto : "no_camera",
                    DriverName = $"{driver.FullName}",
                    DriverPhoto = vehicle != null ? vehicle.DriversPhoto : "no_camera",
                    Url = "",
                    Type = PinType.Place,
                    Position = new Position(Convert.ToDouble(Globals.lastlatitude), Convert.ToDouble(Globals.lastlongitude)),
                    Label = $"Driver's Location",
                    Address = $"Coordinate's: {Globals.lastlatitude}, {Globals.lastlongitude}"
                };
                Device.BeginInvokeOnMainThread(() =>
                {
                    customMap.Pins.Add(driverPin);
                    customPinsList.Add(driverPin);
                });
                foreach (var position in viewModel.OngoingOrders.ToList())
                {
                    CustomPin devicePin = new CustomPin
                    {
                        Name = $"{position.Id}",
                        Url = "",
                        Type = PinType.Place,
                        Position = new Position(Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLat : position.Lat), Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLon : position.Lon)),
                        Label = $"Order: ORD-000{position.Id}",
                        Address = $"Title: {position.AddressTitle}, Address: {position.Address}"
                    };
                    pos.Add(new Position(Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLat : position.Lat), Convert.ToDouble(position.IsChangeAddressAccepted ? position.ChangeAddressLon : position.Lon)));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        customMap.Pins.Add(devicePin);
                    });
                    customPinsList.Add(devicePin);
                    break;

                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    customMap.CustomPins = customPinsList;
                });
                var googleDirection = await JsonWebApiAction.GetMapsDirection(pos.FirstOrDefault(), pos.LastOrDefault(), pos);
                //var googleDirection = await API.SendGetAsyncRequest<GoogleDirection>($"api/Location/Get/Maps?origin={locations[0].Latitude}, {locations[0].Longitude}&destination={locations[1].Latitude}, {locations[1].Longitude}");

                if (googleDirection != null)
                {
                    try
                    {
                        var durations = googleDirection.routes.First().legs;
                        int totalDurationInMinutes = 0;
                        foreach (var leg in durations)
                        {
                            var durationText = leg.duration.text.ToString();
                            totalDurationInMinutes += StrHelper.ComputeDurationInMinutes(durationText);
                        }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            viewModel.ETA = totalDurationInMinutes + " min(s)";
                        });
                    }
                    catch(Exception e)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            viewModel.ETA = "Calculating";
                        });
                    }
                    var polylinePositions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.routes.First().overview_polyline.points)));

                    var polyline = new Xamarin.Forms.Maps.Polyline
                    {
                        StrokeColor = Xamarin.Forms.Color.Blue,
                        StrokeWidth = 15
                    };
                    foreach (var p in polylinePositions.ToList())
                    {
                        polyline.Geopath.Add(p);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        customMap.MapElements.Clear();
                        customMap.MapElements.Add(polyline);
                        customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Globals.lastlatitude, Globals.lastlongitude), Xamarin.Forms.Maps.Distance.FromMiles(0.3)));
                        viewModel.IsBusy = false;
                    });
                }
            }
            catch (InvalidOperationException exception)
            {

            }
            catch (Exception ex)
            {
                viewModel.IsBusy = false;
                //await DisplayAlert("Error", ex.Message, "OK").ConfigureAwait(true);
            }
        }

        

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (viewModel == null) return;
            viewModel.MapImage = customMap.MapType != MapType.Street ? "satellite" : "street_view";
            customMap.MapType = customMap.MapType == MapType.Street ? MapType.Satellite : MapType.Street;
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}