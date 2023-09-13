using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace DineDash.Interface
{
    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string DriverName { get; set; }
        public string DriverPhoto { get; set; }
        public string VehicleName { get; set; }
        public string VehiclePhoto { get; set; }
        public string Url { get; set; }
    }
}
