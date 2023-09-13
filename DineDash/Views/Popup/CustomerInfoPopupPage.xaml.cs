using DineDash.Models;
using DineDash.ViewModels;
using GoogleApi.Entities.Translate.Common.Enums;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerInfoPopupPage : PopupPage
    {
        public CustomerInfoPopupPage(CustomerDetails model)
        {
            InitializeComponent();
            BindingContext = new CustomerInfoPopupViewModel(model);
        }
    }
}