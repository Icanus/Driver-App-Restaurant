﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceOfflineView : ContentView
    {
        public DeviceOfflineView()
        {
            InitializeComponent();
        }
    }
}