using DineDash.Models;
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
    public partial class OrderDetailPage : ContentPage
    {

        OrderDetailViewModel viewModel;
        public string OrderId { get; set; }
        public OrderParameter Order { get; set; }
        public OrderDetailPage(bool IsAcceptOrderView = true)
        {
            InitializeComponent();
            viewModel = new OrderDetailViewModel(IsAcceptOrderView);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Order?.OrderId != null)
            {
                viewModel.Order = Order;
            }
        }
    }
}