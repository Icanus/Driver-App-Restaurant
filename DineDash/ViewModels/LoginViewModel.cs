using DineDash.Interface;
using DineDash.Models;
using DineDash.Utilities;
using DineDash.Views;
using DineDash.Views.Popup;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        IGoogleManager _googleManager => DependencyService.Get<IGoogleManager>();
        public Command LoginCommand { get; }
        public Command SignUpCommand { get; }
        public Command CloseCommand { get; }
        public Command UsernameTextChanged { get; }
        public Command PasswordTextChanged { get; }
        public Command ContinueWithGoogle { get; }
        private bool _IsPassword = true;
        public bool IsPassword
        {
            get
            {
                return _IsPassword;
            }
            set
            {
                _IsPassword = value;
                OnPropertyChanged("IsPassword");
            }
        }
        public ICommand ToggleIsPassword => new Command(() =>
        {
            IsPassword = !IsPassword;
            MessagingCenter.Unsubscribe<object>(this, "LoginPasswordEntryFocus");
            MessagingCenter.Send<object>(this, "LoginPasswordEntryFocus");
        });
        bool isLoginEnabled = false;
        public bool IsLoginEnabled
        {
            get => isLoginEnabled;
            set
            {
                isLoginEnabled = value;
                OnPropertyChanged("IsLoginEnabled");
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        public LoginViewModel()
        {
            IsLoginEnabled = false;
            SignUpCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var signUp = new SignUpPage();
                    signUp.OperationCompleted += SignUp_OperationCompleted;
                    await Navigation.PushAsync(signUp, true);
                });
            });
            CloseCommand = new Command(() =>
            {
                try
                {
                    Navigation.PopAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            LoginCommand = new Command(async () =>
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(400);
                    var driver = await App.RestaurantDatabase.GetDriverAsync(Email, Password);
                    //var customer = await service.GetCustomerAsync(Globals.LoggedCustomerId);
                    string jsonString = JsonConvert.SerializeObject(driver);
                    bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();

                    if (isAvailable)
                    {
                        var userInfo = await JsonWebApiAction.CheckUserInfo(Email, Password);
                        if (userInfo?.Email != null)
                        {
                            driver = userInfo;
                            await App.RestaurantDatabase.UpdateDriverAsync(driver);
                        }
                        else
                        {
                            DisplayAlert("wrong password combination");
                            IsBusy = false;
                            return;
                        }
                    }
                    if (driver?.Email == null)
                    {
                        if (!isAvailable)
                        {
                            DisplayAlert("cannot validate if your account exists since you don't have an internet access.");
                            IsBusy = false;
                            return;
                        }
                        DisplayAlert("wrong password combination. ");
                        IsBusy = false;
                        return;
                    }

                    Globals.LoggedDriverId = driver.DriverId;
                    Globals.IsLogin = true;
                    if (isAvailable)
                    {
                        Globals.IsInitialized = false;
                    }
                    IsBusy = false;
                    if (Device.RuntimePlatform == Device.Android)
                        Globals.IsLoginByGoogle = false;

                    await Navigation.PushAsync(new HomePage());

                }
                catch (Exception e)
                {
                    DisplayAlert("something went wrong. please try again.");
                    IsBusy = false;
                    return;
                }
            });

            UsernameTextChanged = new Command(() =>
            {
                TextChangedCommand();
            });

            PasswordTextChanged = new Command(() =>
            {
                TextChangedCommand();
            });
            ContinueWithGoogle = new Command(async () =>
            {
                IsBusy = true;
                Globals.IsInitialized = true;
                await Task.Delay(300);
                bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();

                if (!isAvailable)
                {
                    DisplayAlert("Kindly Check your internet connection");
                    return;
                }
                _googleManager.Login(OnLoginComplete);
            });
        }

        private async void OnLoginComplete(GoogleUser googleUser, string message)
        {
            IsBusy = true;
            await Task.Delay(300);
            if (googleUser != null)
            {
                //txtName.Text = GoogleUser.Name;
                //txtEmail.Text = GoogleUser.Email;
                //imgProfile.Source = GoogleUser.Picture;
                var userInfo = await JsonWebApiAction.CheckUserInfoByEmail(googleUser.Email);
                if (userInfo?.Email == null)
                {
                    Driver driver = new Driver();
                    var driverId = Guid.NewGuid().ToString();
                    driver.DriverId = driverId;
                    driver.FullName = googleUser.Name;
                    driver.Email = googleUser.Email;
                    driver.CountryCode = "679";
                    driver.Country = "Fiji";
                    driver.Phone = null;
                    driver.DateOfBirth = DateTime.Now;
                    driver.Gender = "Male";
                    driver.AccountPreferences = null;
                    driver.Image = googleUser.Picture.AbsoluteUri.ToString();
                    driver.Password = null;
                    driver.Username = "user-001";
                    driver.TermsAndCondition = true;
                    driver.ReferralCode = null;
                    var result = await JsonWebApiAction.CreateAccount(driver);
                    if (result == (int)Enums.CreationStatusEnums.Error)
                    {
                        DisplayAlert("Something went wrong!");
                        IsBusy = false;
                        return;
                    }
                    var userInfo1 = await JsonWebApiAction.CheckUserInfoByEmail(driver.Email);
                    var res = await App.RestaurantDatabase.UpdateDriverAsync(userInfo1);
                    Console.WriteLine(res);
                    Globals.LoggedDriverId = driver.DriverId;
                    Globals.IsLogin = true;
                    Globals.IsLoginByGoogle = true;
                }
                else
                {
                    //DisplayAlert("wrong password combination");
                    await App.RestaurantDatabase.UpdateDriverAsync(userInfo);
                    Globals.LoggedDriverId = userInfo.DriverId;
                    Globals.IsLogin = true;
                    Globals.IsLoginByGoogle = true;
                    bool hasError = false;
                }

                Email = "";
                Password = "";
                await Navigation.PushAsync(new HomePage());
                IsBusy = false;
            }
            else
            {
                IsBusy = false;
                //DisplayAlert(message);
            }
        }

        public async void RedirectLogin()
        {
            IsBusy = true;
            await Task.Delay(300);
            if (Globals.IsOnSleepLogout)
            {
                Email = "";
                Password = "";
                Globals.IsInitialized = false;
                Globals.IsLogin = false;
                Globals.LoggedDriverId = null;
                //Globals.IsDriving = false;
                if (Device.RuntimePlatform == Device.Android)
                {
                    IGoogleManager _googleManager = DependencyService.Get<IGoogleManager>();
                    if (Globals.IsLoginByGoogle)
                    {
                        Globals.IsLoginByGoogle = false;
                        _googleManager.Logout();
                    }
                }
                IsBusy = false;
                Globals.IsOnSleepLogout = false;
                return;
            }
            if (Globals.IsLogin)
            {
                var drvr = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);
                Globals.IsLogin = true;
                Globals.IsInitialized = true;
                await Navigation.PushAsync(new HomePage());
                IsBusy = false;
                return;
            }
            IsBusy = false;
        }

        void TextChangedCommand()
        {
            if (string.IsNullOrEmpty(Email))
            {
                IsLoginEnabled = false;
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                IsLoginEnabled = false;
                return;
            }
            if (Email?.Count() < 3)
            {
                IsLoginEnabled = false;
                return;
            }
            if (Password?.Count() < 3)
            {
                IsLoginEnabled = false;
                return;
            }
            IsLoginEnabled = true;

        }

        private void SignUp_OperationCompleted(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        void DisplayAlert(string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await PopupNavigation.Instance.PushAsync(new InfoPopupPage("Info", message, "Okay"));
            });
        }
    }
}
