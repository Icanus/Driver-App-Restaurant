using DineDash.Enums;
using DineDash.Helpers;
using DineDash.Models;
using DineDash.Resources;
using DineDash.Utilities;
using DineDash.Views.Popup;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class MyAccountViewModel : BaseViewModel
    {
        public Command SignUpCommand { get; }
        public Command LoginCommand { get; }
        public Command TermsCommand { get; }
        public Command RadioCommand { get; }
        public Command AddAddress { get; }
        public Command CloseCommand { get; }
        public Command ShowPopupCommand { get; }
        public ICommand CountrySelectedCommand { get; }
        private CountryModel _selectedCountry;
        public CountryModel SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged("SelectedCountry");
            }
        }
        public ICommand ToggleIsPassword => new Command(() =>
        {
            IsPassword = !IsPassword;
            MessagingCenter.Unsubscribe<object>(this, "MyAccountPasswordEntryFocus");
            MessagingCenter.Send<object>(this, "MyAccountPasswordEntryFocus");
        });

        public ICommand ToggleIsConfirmPassword => new Command(() =>
        {
            IsConfirmPassword = !IsConfirmPassword;
            MessagingCenter.Unsubscribe<object>(this, "MyAccountConfirmPasswordEntryFocus");
            MessagingCenter.Send<object>(this, "MyAccountConfirmPasswordEntryFocus");
        });

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

        private bool _IsConfirmPassword = true;
        public bool IsConfirmPassword
        {
            get
            {
                return _IsConfirmPassword;
            }
            set
            {
                _IsConfirmPassword = value;
                OnPropertyChanged("IsConfirmPassword");
            }
        }

        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        private string username;
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged("Username");
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

        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
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

        private string confirmPassword;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }

        private DateTime dateOfBirth = DateTime.Now;
        public DateTime DateOfBirth
        {
            get => dateOfBirth;
            set
            {
                dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }

        private string gender = "Male";
        public string Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        private string accountPreference;
        public string AccountPreference
        {
            get => accountPreference;
            set
            {
                accountPreference = value;
                OnPropertyChanged("AccountPreference");
            }
        }

        private string address;
        public string CurrentAddress
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged("CurrentAddress");
            }
        }


        private Address userAddress;
        public Address UserAddress
        {
            get => userAddress;
            set
            {
                userAddress = value;
                OnPropertyChanged("UserAddress");
            }
        }

        private string termsAndCondition;
        public string TermsAndCondition
        {
            get => termsAndCondition;
            set
            {
                termsAndCondition = value;
                OnPropertyChanged("TermsAndCondition");
            }
        }
        private string profile;
        public string Profile
        {
            get => profile;
            set
            {
                profile = value;
                OnPropertyChanged("Profile");
            }
        }
        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        private string referralCode;
        public string ReferralCode
        {
            get => referralCode;
            set
            {
                referralCode = value;
                OnPropertyChanged("ReferralCode");
            }
        }

        string appVersion = AppInfo.VersionString;
        public string AppVersion
        {
            get => appVersion;
            set
            {
                appVersion = value;
                OnPropertyChanged("AppVersion");
            }
        }
        bool isPasswordReadOnly = false;
        public bool IsPasswordReadOnly
        {
            get => isPasswordReadOnly;
            set
            {
                isPasswordReadOnly = value;
                OnPropertyChanged("IsPasswordReadOnly");
            }
        }
        async void loadAccountDetails()
        {
            IsBusy = true;
            var model = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);
            Id = model.Id;
            FullName = model.FullName;
            Email = model.Email;
            Phone = model.Phone;
            Gender = model.Gender;
            Password = model.Password;
            if (Globals.IsLoginByGoogle)
                IsPasswordReadOnly = true;
            ConfirmPassword = model.Password;
            ReferralCode = model.ReferralCode;
            Profile = model.Image;
            IsBusy = false;
            var country = string.IsNullOrEmpty(model.Country) ? "Fiji" : model.Country;
            SelectedCountry = CountryUtils.GetCountryModelByName(country);
        }
        public MyAccountViewModel()
        {
            Title = AppResources.SignUp;
            loadAccountDetails();
            SignUpCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(300);

                var current = Connectivity.NetworkAccess;

                if (current != NetworkAccess.Internet)
                {
                    DisplayAlert("- No internet connection", true);
                    IsBusy = false;
                    return;
                }
                bool hasError = false;
                string errorMessage = "";
                if (!Globals.IsLoginByGoogle)
                {
                    if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                    {
                        DisplayAlert("The required fields have not been filled up yet.", true);
                        IsBusy = false;
                        return;
                    }

                    if (Password != ConfirmPassword)
                    {
                        errorMessage += "- The password does not match.\n";
                        hasError = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(FullName))
                    {
                        DisplayAlert("The required fields have not been filled up yet.", true);
                        IsBusy = false;
                        return;
                    }
                }
                if (!IsValidEmail.CheckEmail(Email))
                {
                    errorMessage += "- The Email is Invalid.\n";
                    hasError = true;
                }
                if (hasError)
                {
                    DisplayAlert(errorMessage, true);
                    IsBusy = false;
                    return;
                }
                Driver driver = new Driver();
                driver.DriverId = Globals.LoggedDriverId;
                driver.FullName = FullName;
                driver.Email = Email;
                driver.CountryCode = SelectedCountry.CountryCode;
                driver.Country = SelectedCountry.CountryName;
                driver.Phone = Phone;
                driver.DateOfBirth = DateOfBirth;
                driver.Gender = Gender;
                driver.AccountPreferences = AccountPreference;
                driver.Image = Profile;
                driver.Password = Password;
                driver.Username = "user-001";
                driver.TermsAndCondition = true;
                driver.ReferralCode = ReferralCode;
                var result = await JsonWebApiAction.UpdateAccount(Globals.LoggedDriverId, driver);
                if (result == (int)CreationStatusEnums.AlreadyExist)
                {
                    DisplayAlert("Email Already Exists", true);
                    IsBusy = false;
                    return;
                }
                await App.RestaurantDatabase.UpdateDriverAsync(driver);

                Globals.LoggedDriverId = driver.DriverId;
                Globals.IsLogin = true;

                #region

                MessagingCenter.Unsubscribe<object>(this, "UpdateLoginStatus");
                MessagingCenter.Send<object>(this, "UpdateLoginStatus");
                #endregion

                IsBusy = false;

                DisplayAlert("Successfully updated account");
            });
            AddAddress = new Command(async () =>
            {
                //await Shell.Current.GoToAsync($"{nameof(AddressDetailPage)}");
            });
            RadioCommand = new Command<string>((args) =>
            {
                Gender = args;
            });
            LoginCommand = new Command(async () => await Navigation.PopAsync());
            TermsCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
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
            ShowPopupCommand = new Command(async () => await ExecuteShowPopupCommand());
            CountrySelectedCommand = new Command(country => ExecuteCountrySelectedCommand(country as CountryModel));
        }
        private Task ExecuteShowPopupCommand()
        {
            var popup = new ChooseCountryPopup(SelectedCountry)
            {
                CountrySelectedCommand = CountrySelectedCommand
            };
            return Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
        }

        private void ExecuteCountrySelectedCommand(CountryModel country)
        {
            SelectedCountry = country;
        }
        void DisplayAlert(string message, bool isError = false)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!isError)
                {
                    var infopage = new InfoPopupPage("Info", message, "Okay");
                    await PopupNavigation.Instance.PushAsync(infopage);
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new InfoPopupPage("Info", message, "Okay", true));
                }
            });
        }
    }
}
