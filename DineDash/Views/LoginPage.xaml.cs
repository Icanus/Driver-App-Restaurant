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
    public partial class LoginPage : ContentPage
    {
        private SignalRService _signalRService;
        public static bool IsInitialize = false;
        LoginViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();
            //_signalRService = new SignalRService();
            viewModel = new LoginViewModel();
            BindingContext = viewModel;
            Communication();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RedirectLogin();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        void Communication()
        {
            MessagingCenter.Unsubscribe<object>(this, "LoginPasswordEntryFocus");
            MessagingCenter.Subscribe<object>(this, "LoginPasswordEntryFocus", (sender) =>
            {
                PasswordEntry.Focus();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        PasswordEntry.CursorPosition = PasswordEntry.Text.Length;
                    }
                    catch (Exception) { }
                });
            });
        }
    }
}