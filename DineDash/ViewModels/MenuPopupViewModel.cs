using DineDash.Helpers;
using DineDash.Interface;
using DineDash.Utilities;
using DineDash.Views;
using DineDash.Views.Popup;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    internal class MenuPopupViewModel : BaseViewModel
    {
        public Command NavigateToWalletHistory { get; }
        public Command MyAccountPageCommand { get; }
        public Command NavigateToOrdersPageCommand { get; }
        public Command LoginCommand { get; }
        public Command LogoutCommand { get; }
        public Command NavigateAddressPageCommand { get; }
        public Command ReferralPageCommand { get; }
        public Command FeedbackCommand { get; }
        public Command NavigateToVehicleDetailsPageCommand { get; }

        bool isLogin = Globals.IsLogin;
        public bool IsLogin
        {
            get => isLogin;
            set
            {
                isLogin = value;
                OnPropertyChanged("IsLogin");
            }
        }

        bool isNotLogin = !Globals.IsLogin;
        public bool IsNotLogin
        {
            get => isNotLogin;
            set
            {
                isNotLogin = value;
                OnPropertyChanged("IsNotLogin");
            }
        }

        string balance;
        public string Balance
        {
            get => balance;
            set
            {
                balance = value;
                OnPropertyChanged("Balance");
            }
        }

        bool isActiveMember = false;
        public bool IsActiveMember
        {
            get => isActiveMember;
            set
            {
                isActiveMember = value;
                OnPropertyChanged("IsActiveMember");
            }
        }

        bool isNotActiveMember = false;
        public bool IsNotActiveMember
        {
            get => isNotActiveMember;
            set
            {
                isNotActiveMember = value;
                OnPropertyChanged("IsNotActiveMember");
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
        string initials;
        public string Initials
        {
            get => initials;
            set
            {
                initials = value;
                OnPropertyChanged("Initials");
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
        bool isPressedAlready = false;
        public MenuPopupViewModel()
        {
            UpdateLoginStatus();

            MyAccountPageCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(300);
                CloseMenu();
                await Navigation.PushAsync(new MyAccountPage());
                IsBusy = false;
            });
            NavigateToOrdersPageCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(300);
                CloseMenu();
                await Navigation.PushAsync(new OrdersPage());
            });
            LoginCommand = new Command(async() =>
            {
                IsBusy = true;
                await Task.Delay(300);
                CloseMenu();
                MessagingCenter.Unsubscribe<object>(this, "DisplayLogin");
                MessagingCenter.Send<object>(this, "DisplayLogin");
                IsBusy = false;
            });
            LogoutCommand = new Command(async() =>
            {
                IsBusy = true;
                await Task.Delay(300);
                CloseMenu();
                MessagingCenter.Unsubscribe<object>(this, "LogoutCommand");
                MessagingCenter.Send<object>(this, "LogoutCommand");
                IsBusy = false;

            });


            NavigateToVehicleDetailsPageCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(300);
                CloseMenu();
                await Navigation.PushAsync(new VehicleDetailsPage());
                IsBusy = false;
                IsBusy = false;

            });
        }
        void DisplayAlert(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await PopupNavigation.Instance.PushAsync(new InfoPopupPage("Info", message, "Okay"));
            });
        }

        void CloseMenu()
        {
            MessagingCenter.Unsubscribe<object>(this, "UpdateLoginStatus");
            MessagingCenter.Unsubscribe<object>(this, "CloseMenu");
            MessagingCenter.Send<object>(this, "CloseMenu");
        }
        bool IsAlreadyRunningUpdateLogin = false;
        async void UpdateLoginStatus()
        {
            IsBusy = true;
            await Task.Delay(300);
            if (Globals.IsLogin == false)
            {
                IsNotLogin = true;
                IsLogin = false;
            }
            else
            {
                IsLogin = true;
                IsNotLogin = false;
            }

            IsLogin = Globals.IsLogin;
            IsNotLogin = !IsLogin;

            var customer = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);

            var vehicle = await App.RestaurantDatabase.GetVehicleAsync(Globals.LoggedDriverId);
            Device.BeginInvokeOnMainThread(() =>
            {
                FullName = customer?.FullName;
                Initials = StrHelper.GetInitials(string.IsNullOrEmpty(customer?.FullName) ? "" : customer?.FullName);
                ImageFile = vehicle != null ? vehicle.DriversPhoto : "no_camera";
                if (Globals.IsLoginByGoogle)
                {
                    try
                    {
                        bool isAvailableInternet = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                        if (isAvailableInternet)
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
                    catch (Exception e)
                    {

                    }
                }
            });

            IsBusy = false;
        }

        public void Communication()
        {
            MessagingCenter.Unsubscribe<object>(this, "UpdateLoginStatus");
            MessagingCenter.Subscribe<object>(this, "UpdateLoginStatus", (args) =>
            {
                UpdateLoginStatus();
            });
        }
    }
}
