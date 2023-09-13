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
    public partial class AcceptOrderPage : ContentPage
    {
        public AcceptOrderPage(string orderId)
        {
            InitializeComponent();
            BindingContext = new AcceptOrderViewModel(orderId);
        }
    }
}