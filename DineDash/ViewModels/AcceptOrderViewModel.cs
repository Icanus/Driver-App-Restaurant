using DineDash.Models;
using DineDash.Utilities;
using DineDash.Views.Snackbar;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class AcceptOrderViewModel : BaseViewModel
    {
        MediaFile _mediaFile;
        public Command UploadImage { get; }
        public Command SubmitCommand { get; }
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
        ImageSource imageFile;
        public ImageSource ImageFile
        {
            get => imageFile;
            set
            {
                imageFile = value;
                OnPropertyChanged("ImageFile");
            }
        }
        string _orderId { get; set; }
        public AcceptOrderViewModel(string orderId)
        {
            _orderId = orderId;
            UploadImage = new Command(async() =>
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
                                DisplayAlert("No Camera", ":( No camera available.", "OK");
                                return;
                            }
                            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                            {
                                SaveToAlbum = false,
                                CompressionQuality = 40,
                                CustomPhotoSize = 35,
                                PhotoSize = PhotoSize.MaxWidthHeight,
                                MaxWidthHeight = 2000
                            });
                            if (photo == null)
                            {
                                IsBusy = false;
                                return;
                            }
                            ImageFile = photo.Path;
                            ProfileString = photo.Path;
                            break;
                        case "Gallery":
                            await CrossMedia.Current.Initialize();
                            _mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                            {
                                CompressionQuality = 40,
                                CustomPhotoSize = 35,
                                PhotoSize = PhotoSize.MaxWidthHeight,
                                MaxWidthHeight = 2000
                            });
                            if (_mediaFile == null)
                            {
                                IsBusy = false;
                                return;
                            }
                            ImageFile = _mediaFile.Path;
                            ProfileString = _mediaFile.Path;
                            break;
                        default:
                            break;
                    }
                    IsBusy = false;
                });
            });
            SubmitCommand = new Command(async () =>
            {
                if (string.IsNullOrEmpty(ProfileString))
                {
                    DisplayAlert("No image was selected.", Color.Orange);
                    return;
                }
                var url = await getUploadedImageString(ProfileString);
                OrderDocuments docs = new OrderDocuments
                {
                    OrderId = _orderId,
                    OnTheWayImage = url
                };
                var res = await JsonWebApiAction.UpdateOrderDocuments(_orderId, 0, docs);
                if(res != 0)
                {

                }
            });
        }

        void DisplayAlert(string message, Color BG)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ToastSnackbar.DisplaySnackbar(CurrentPage, $"{message}", 2, BG);
            });
        }
        async Task<string> getUploadedImageString(string imagePath)
        {
            using (var fileStream = File.OpenRead(imagePath))
            {
                var account = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName=efldocstore;AccountKey={App.azureKeys};EndpointSuffix=core.windows.net");
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("images");
                await container.CreateIfNotExistsAsync();
                var name = Guid.NewGuid().ToString();
                var blockBlob = container.GetBlockBlobReference($"{name}.png");
                await blockBlob.UploadFromStreamAsync(fileStream);
                string url = blockBlob.Uri.OriginalString;
                return url;
            }

            return "";
        }

        async Task<string> deleteUploadedImageString(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "";
            var account = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName=efldocstore;AccountKey={App.azureKeys};EndpointSuffix=core.windows.net");
            var client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("images");

            Uri uri = new Uri(imagePath);
            var fileName = Path.GetFileName(uri.LocalPath);
            CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
            var res = await blob.DeleteIfExistsAsync();
            return res.ToString();
        }

    }
}
