﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DineDash.Controls
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
