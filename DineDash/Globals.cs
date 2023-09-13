using DineDash.Models;
using DineDash.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Essentials;

namespace DineDash
{
    public class Globals
    {
        public static string LoggedDriverId
        {
            get
            {
                if (Preferences.ContainsKey(nameof(LoggedDriverId)))
                    return JsonConvert.DeserializeObject<string>(Preferences.Get(nameof(LoggedDriverId), null));
                return string.Empty;
            }
            set => Preferences.Set(nameof(LoggedDriverId), JsonConvert.SerializeObject(value));
        }

        public static bool IsLogin
        {
            get
            {
                if (Preferences.ContainsKey(nameof(IsLogin)))
                    return JsonConvert.DeserializeObject<bool>(Preferences.Get(nameof(IsLogin), null));
                return false;
            }
            set => Preferences.Set(nameof(IsLogin), JsonConvert.SerializeObject(value));
        }

        public static bool IsInitialized
        {
            get
            {
                if (Preferences.ContainsKey(nameof(IsInitialized)))
                    return JsonConvert.DeserializeObject<bool>(Preferences.Get(nameof(IsInitialized), null));
                return false;
            }
            set => Preferences.Set(nameof(IsInitialized), JsonConvert.SerializeObject(value));
        }

        public static double lastlatitude { get; set; }
        public static double lastlongitude { get; set; }
        public static bool IsLoginByGoogle
        {
            get
            {
                if (Preferences.ContainsKey(nameof(IsLoginByGoogle)))
                    return JsonConvert.DeserializeObject<bool>(Preferences.Get(nameof(IsLoginByGoogle), null));
                return false;
            }
            set => Preferences.Set(nameof(IsLoginByGoogle), JsonConvert.SerializeObject(value));
        }
        public static string StoreLat = "-18.112000";
        public static string StoreLon = "178.468600";

        public static ObservableCollection<OrderParameter> OngoingOrders { get; set; }
        //public static bool IsOnTheWayInitialize
        //{
        //    get
        //    {
        //        if (Preferences.ContainsKey(nameof(IsOnTheWayInitialize)))
        //            return JsonConvert.DeserializeObject<bool>(Preferences.Get(nameof(IsOnTheWayInitialize), null));
        //        return false;
        //    }
        //    set => Preferences.Set(nameof(IsOnTheWayInitialize), JsonConvert.SerializeObject(value));
        //}

        public static int lastSyncId
        {
            get
            {
                if (Preferences.ContainsKey(nameof(lastSyncId)))
                    return JsonConvert.DeserializeObject<int>(Preferences.Get(nameof(lastSyncId), null));
                return 0;
            }
            set => Preferences.Set(nameof(lastSyncId), JsonConvert.SerializeObject(value));
        }
        private const string AppWentToSleepDateTimeKey = "AppWentToSleepDateTime";
        public static string AppWentToSleepDateTime
        {
            get
            {
                if (Preferences.ContainsKey(nameof(AppWentToSleepDateTimeKey)))
                    return JsonConvert.DeserializeObject<string>(Preferences.Get(nameof(AppWentToSleepDateTimeKey), null));
                return string.Empty;
            }
            set => Preferences.Set(nameof(AppWentToSleepDateTimeKey), JsonConvert.SerializeObject(value));
        }
        public static bool IsOnSleepLogout
        {
            get
            {
                if (Preferences.ContainsKey(nameof(IsOnSleepLogout)))
                    return JsonConvert.DeserializeObject<bool>(Preferences.Get(nameof(IsOnSleepLogout), null));
                return false;
            }
            set => Preferences.Set(nameof(IsOnSleepLogout), JsonConvert.SerializeObject(value));
        }
        public static string LastAppUsageDateTime
        {
            get
            {
                if (Preferences.ContainsKey(nameof(LastAppUsageDateTime)))
                    return JsonConvert.DeserializeObject<string>(Preferences.Get(nameof(LastAppUsageDateTime), null));
                return string.Empty;
            }
            set => Preferences.Set(nameof(LastAppUsageDateTime), JsonConvert.SerializeObject(value));
        }
    }
}
