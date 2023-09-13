﻿using DineDash.ViewModels;
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
    public partial class SignUpPage : ContentPage
    {
        SignUpViewModel viewModel;
        public event EventHandler<EventArgs> OperationCompleted;
        public SignUpPage()
        {
            InitializeComponent();
            viewModel = new SignUpViewModel();
            BindingContext = viewModel;
            Communication();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OperationCompleted += OperationCompleted;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.OperationCompleted -= OperationCompleted;
        }

        void Communication()
        {
            MessagingCenter.Unsubscribe<object>(this, "SignupPasswordEntryFocus");
            MessagingCenter.Subscribe<object>(this, "SignupPasswordEntryFocus", (sender) =>
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

            MessagingCenter.Unsubscribe<object>(this, "SignupConfirmPasswordEntryFocus");
            MessagingCenter.Subscribe<object>(this, "SignupConfirmPasswordEntryFocus", (sender) =>
            {
                ConfirmPasswordEntry.Focus();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        ConfirmPasswordEntry.CursorPosition = ConfirmPasswordEntry.Text.Length;
                    }
                    catch (Exception) { }
                });
            });
        }
    }
}