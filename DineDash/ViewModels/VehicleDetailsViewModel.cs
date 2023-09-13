using DineDash.Helpers;
using DineDash.Interface;
using DineDash.Models;
using DineDash.Utilities;
using DineDash.Views.Popup;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public class VehicleDetailsViewModel : BaseViewModel
    {
        MediaFile _mediaFile;
        public Command UploadImage { get; }
        public Command UploadCarImage { get; }
        public Command SubmitCommand { get; }
        public Command CloseCommand { get; }
        bool isProfileChange = false;
        bool isCarPhotoChange = false;
        string _ProfileString;
        public string ProfileString
        {
            get => _ProfileString;
            set
            {
                _ProfileString = value;
                OnPropertyChanged("ProfileString");
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
        string _CarDescription;
        public string CarDescription
        {
            get => _CarDescription;
            set
            {
                _CarDescription = value;
                OnPropertyChanged("CarDescription");
            }
        }
        string _CarRegistration;
        public string CarRegistration
        {
            get => _CarRegistration;
            set
            {
                _CarRegistration = value;
                OnPropertyChanged("CarRegistration");
            }
        }

        string _CarImageString;
        public string CarImageString
        {
            get => _CarImageString;
            set
            {
                _CarImageString = value;
                OnPropertyChanged("CarImageString");
            }
        }
        bool _IsLabelStringVisible = true;
        public bool IsLabelStringVisible
        {
            get => _IsLabelStringVisible;
            set
            {
                _IsLabelStringVisible = value;
                OnPropertyChanged("IsLabelStringVisible");
            }
        }
        
        ImageSource carImageFile;
        public ImageSource CarImageFile
        {
            get => carImageFile;
            set
            {
                carImageFile = value;
                OnPropertyChanged("CarImageFile");
            }
        }
        public VehicleDetailsViewModel()
        {
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
            UploadImage = new Command(async () =>
            {
                if (Globals.IsLoginByGoogle) return;
                InitializeCamera(true);
            });
            UploadCarImage = new Command(async () =>
            {
                InitializeCamera(false);
            });
            SubmitCommand = new Command(async () =>
            {
                IsBusy = true;
                await Task.Delay(300);
                bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();

                if (!isAvailable)
                {
                    DisplayAlert("Check your internet connection");
                    IsBusy = false;
                    return;
                }
                if (string.IsNullOrEmpty(ProfileString))
                {

                    if (!Globals.IsLoginByGoogle)
                    {
                        DisplayAlert("Driver's image is required");
                        IsBusy = false;
                        return;
                    }
                }
                if (string.IsNullOrEmpty(CarImageString))
                {
                    DisplayAlert("Vehicle's image is required");
                    IsBusy = false;
                    return;
                }
                if (string.IsNullOrEmpty(CarRegistration))
                {
                    DisplayAlert("Vehicle's registration is required");
                    IsBusy = false;
                    return;
                }
                string carPhoto = string.Empty;
                string driversPhoto = string.Empty;
                try
                {
                    var vehicleDetails = await App.RestaurantDatabase.GetVehicleAsync(Globals.LoggedDriverId);
                    if(!string.IsNullOrEmpty(vehicleDetails?.DriverId))
                    {
                        if (isCarPhotoChange)
                        {
                            await StrHelper.deleteUploadedImageString(vehicleDetails.CarPhoto);
                        }
                        if (isProfileChange)
                        {
                            await StrHelper.deleteUploadedImageString(vehicleDetails.DriversPhoto);
                        }
                    }
                    carPhoto = isCarPhotoChange ? await StrHelper.getUploadedImageString(CarImageString) : vehicleDetails.CarPhoto;
                    driversPhoto = isProfileChange ? await StrHelper.getUploadedImageString(ProfileString) : vehicleDetails.DriversPhoto;
                    if (Globals.IsLoginByGoogle)
                    {
                        var customer = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);
                        driversPhoto = customer?.Image;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    DisplayAlert("Something went wrong, please try again.");
                    IsBusy = false;
                    return;
                }
                Vehicle vehicle = new Vehicle
                {
                    Id = 0,
                    CarPhoto = carPhoto,
                    CarRegistration = CarRegistration,
                    CarDescription = CarDescription,
                    DriverId = Globals.LoggedDriverId,
                    DriversPhoto = driversPhoto,
                };
                var res = await JsonWebApiAction.UpsertVehicle(vehicle);
                if(res == 1)
                {
                    var details = await JsonWebApiAction.GetVehicleByDriverId(Globals.LoggedDriverId);
                    await App.RestaurantDatabase.UpsertVehicle(details);
                    initialize(details);
                    DisplayAlert("Successfully updated vehicle");
                    initializeValues();
                }
                else if(res == 0)
                {
                    try
                    {
                        await StrHelper.deleteUploadedImageString(carPhoto);
                        await StrHelper.deleteUploadedImageString(driversPhoto);
                    }
                    catch(Exception e)
                    {

                    }
                    DisplayAlert("Something went wrong, please try again.");
                    IsBusy = false;
                }
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

        async void InitializeCamera(bool isProfile = true)
        {
            IsBusy = true;
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var action = await App.Current.MainPage.DisplayActionSheet("Options: ", "Cancel", null, "Camera", "Gallery");
                switch (action)
                {
                    case "Camera":
                        await CrossMedia.Current.Initialize();
                        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                            });
                            return;
                        }
                        var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            SaveToAlbum = false,
                            CompressionQuality = 40,
                            CustomPhotoSize = 35,
                            PhotoSize = PhotoSize.Full,
                            MaxWidthHeight = 2000
                        });
                        if (photo == null)
                        {
                            IsBusy = false;
                            return;
                        }

                        if (isProfile)
                        {
                            ImageFile = photo.Path;
                            ProfileString = photo.Path;
                            isProfileChange = true;
                        }
                        else
                        {
                            CarImageFile = photo.Path;
                            CarImageString = photo.Path;
                            isCarPhotoChange = true;
                        }
                        break;
                    case "Gallery":
                        await CrossMedia.Current.Initialize();
                        _mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            CompressionQuality = 40,
                            CustomPhotoSize = 35,
                            PhotoSize = PhotoSize.Full,
                            MaxWidthHeight = 2000
                        });
                        if (_mediaFile == null)
                        {
                            IsBusy = false;
                            return;
                        }

                        if (isProfile)
                        {
                            ImageFile = _mediaFile.Path;
                            ProfileString = _mediaFile.Path;
                            isProfileChange = true;
                        }
                        else
                        {
                            CarImageFile = _mediaFile.Path;
                            CarImageString = _mediaFile.Path;
                            isCarPhotoChange = true;
                        }
                        break;
                    default:
                        break;
                }
                IsBusy = false;
            });
        }

        public async void initializeValues()
        {
            var res = await App.RestaurantDatabase.GetVehicleAsync(Globals.LoggedDriverId);
            if(string.IsNullOrEmpty(res.DriverId))
            {
                bool isAvailable = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                if (isAvailable)
                {
                    res = await JsonWebApiAction.GetVehicleByDriverId(Globals.LoggedDriverId);
                    if (!string.IsNullOrEmpty(res.DriverId))
                    {
                        initialize(res);
                        await App.RestaurantDatabase.UpsertVehicle(res);
                    }
                }
            }
            else
            {
                initialize(res);
            }
            if (Globals.IsLoginByGoogle)
            {
                try
                {
                    bool isAvailableInternet = DependencyService.Get<INetworkAvailable>().IsNetworkAvailable();
                    if (isAvailableInternet)
                    {
                        byte[] bytes;
                        var customer = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);
                        ProfileString = customer.Image;
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
                IsLabelStringVisible = false;
            }
        }

        async void initialize(Vehicle res)
        {
            ImageFile = res.DriversPhoto;
            CarDescription = res.CarDescription;
            CarRegistration = res.CarRegistration;
            CarImageFile = res.CarPhoto;
            ProfileString = res.DriversPhoto;
            if (Globals.IsLoginByGoogle)
            {
                try
                {
                    byte[] bytes;
                    var customer = await App.RestaurantDatabase.GetDriverAsync(Globals.LoggedDriverId);
                    ProfileString = customer.Image;
                    using (WebClient client = new WebClient())
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        bytes = client.DownloadData(customer.Image);
                    }
                    Stream stream = new MemoryStream(bytes);
                    ImageFile = ImageSource.FromStream(() => { return stream; });
                }
                catch (Exception e)
                {

                }
            }
            CarImageString = res.CarPhoto;
            isProfileChange = false;
            isCarPhotoChange = false;
        }
    }
}
