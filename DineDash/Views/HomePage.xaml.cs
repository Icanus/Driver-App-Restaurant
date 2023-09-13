using DineDash.Services;
using DineDash.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        //private SignalRService _signalRService;
        public static bool IsInitialize = false;
        HomeViewModel viewModel;
        public HomePage()
        {
            InitializeComponent();
            //_signalRService = new SignalRService();
            viewModel = new HomeViewModel();
        }

        private async void ConnectSignalR()
        {
            if (IsInitialize)
                return;
            //await _signalRService.ReconnectAsync();
            IsInitialize = true;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext == null)
            {
                BindingContext = viewModel;
            }
            viewModel.OnAppearing();
            //ConnectSignalR();
            //await _signalRService.FetchAndSendRecordsByDriverId(Globals.LoggedDriverId);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}