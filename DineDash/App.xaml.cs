using DineDash.Data;
using DineDash.Helpers;
using DineDash.Interface;
using DineDash.Utilities;
using DineDash.Views;
using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash
{
    public partial class App : Application
    {
        public static string APIUrl = "https://fooddelappapi.azurewebsites.net/";
        public static string googleApiUrl = "https://maps.googleapis.com/maps/";
        public static string GOOGLE_MAP_API_KEY = "AIzaSyBO0zK86DSHfJ1p7ZuaiVkClqCXLvUhpIY";
        public static string azureKeys = "wguLxop0MASXyxPTYXE30xuElCiQfrenuriHmAPgnN2iIJnacYrWvv9nBcizO17F651DqAhow60O+AStlCOAiw==";
        public static IJsonWebApiAgent jsonWebApiAgent;
        private static RestaurantDatabase restaurantDatabase;
        public static int timeOut = 30;

        public static RestaurantDatabase RestaurantDatabase
        {
            get
            {
                if (restaurantDatabase == null)
                {
                    restaurantDatabase = new RestaurantDatabase();
                }
                return restaurantDatabase;
            }
        }
        public App()
        {
            InitializeComponent();
            try
            {
                OnInitialized();
                jsonWebApiAgent = new JsonWebApiAgent();

                TimeSpan totalTimeSlept = DateTime.Now - DateTime.Parse(Globals.LastAppUsageDateTime);
                bool isLogin = false;
                if (Globals.IsLogin)
                    isLogin = true;
                if (Globals.IsLoginByGoogle)
                    isLogin = true;
                if (totalTimeSlept.Minutes >= timeOut && isLogin)
                {
                    Globals.IsOnSleepLogout = true;
                }
            }
            catch (Exception)
            {

            }
            MainPage = new NavigationPage(new LoginPage());
        }
        private async void InitializeAppAsync()
        {
            bool isNewInstallation = await InstallationHelper.IsNewInstallationAsync();

            if (isNewInstallation)
            {
                await InstallationHelper.PerformNewInstallationTasksAsync();
                Globals.IsInitialized = false;
            }

            // Continue with your app initialization
            // ...
        }
        async void OnInitialized()
        {
            await RestaurantDatabase.IsTableExists("Driver");
            //InitializeAppAsync();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Globals.AppWentToSleepDateTime = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
        }

        protected override void OnResume()
        {
            TimeSpan totalTimeSlept = DateTime.Now - DateTime.Parse(Globals.AppWentToSleepDateTime);
            bool isLogin = false;
            if (Globals.IsLogin)
                isLogin = true;
            if(Globals.IsLoginByGoogle)
                isLogin = true;
            if (totalTimeSlept.Minutes >= timeOut && isLogin)
            {
                Globals.IsOnSleepLogout = true;
                MessagingCenter.Unsubscribe<object>(this, "OnSleepLogout");
                MessagingCenter.Send<object>(this, "OnSleepLogout");
            }
        }
    }
}
