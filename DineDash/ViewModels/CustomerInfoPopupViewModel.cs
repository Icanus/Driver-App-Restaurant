using DineDash.Models;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DineDash.ViewModels
{
    public class CustomerInfoPopupViewModel : BaseViewModel
    {
        public Command CloseCommand { get; }
        CustomerDetails _CustomerDetails;
        public CustomerDetails CustomerInfo
        {
            get => _CustomerDetails;
            set
            {
                _CustomerDetails = value;
                OnPropertyChanged("CustomerInfo");
            }
        }
        public CustomerInfoPopupViewModel(CustomerDetails res)
        {
            CloseCommand = new Command(async () =>
            {
                try
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception ex)
                {

                }
            });
            LoadDetails(res);
        }
        void LoadDetails(CustomerDetails res)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                CustomerInfo = res;
            });
        }
    }
}
