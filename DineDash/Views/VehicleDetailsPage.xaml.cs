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
    public partial class VehicleDetailsPage : ContentPage
    {
        VehicleDetailsViewModel viewModel;
        public VehicleDetailsPage()
        {
            InitializeComponent();
            viewModel = new VehicleDetailsViewModel();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.initializeValues();
        }
    }
}