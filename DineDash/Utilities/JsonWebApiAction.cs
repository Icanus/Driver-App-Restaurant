using DineDash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace DineDash.Utilities
{
    public class JsonWebApiAction
    {
        public static async Task<Driver> CheckUserInfo(string email, string password)
        {
            try
            {
                var account = new
                {
                    Email = email,
                    Password = password
                };

                return await App.jsonWebApiAgent.SendGetAsyncRequest<Driver>($"/api/Driver/Email/{email}/Password/{password}");
            }
            catch (Exception e)
            {
                return new Driver();
            }
        }

        public static async Task<int> CreateAccount(Driver driver)
        {
            try
            {
                var account = new
                {
                    id = 0,
                    driverId = driver.DriverId,
                    fullName = string.IsNullOrEmpty(driver.FullName) ? "" : driver.FullName,
                    username = string.IsNullOrEmpty(driver.Username) ? "" : driver.Username,
                    email = string.IsNullOrEmpty(driver.Email) ? "" : driver.Email,
                    password = string.IsNullOrEmpty(driver.Password) ? "" : driver.Password,
                    phone = string.IsNullOrEmpty(driver.Phone) ? "" : driver.Phone,
                    dateOfBirth = "2023-07-25T07:53:40.399Z",
                    gender = string.IsNullOrEmpty(driver.Gender) ? "" : driver.Gender,
                    accountPreferences = string.IsNullOrEmpty(driver.AccountPreferences) ? "" : driver.AccountPreferences,
                    termsAndCondition = driver.TermsAndCondition,
                    image = string.IsNullOrEmpty(driver.Image) ? "" : driver.Image,
                    referralCode = string.IsNullOrEmpty(driver.ReferralCode) ? "" : driver.ReferralCode,
                    country = string.IsNullOrEmpty(driver.Country) ? "Fiji" : driver.Country,
                    countryCode = string.IsNullOrEmpty(driver.CountryCode) ? "679" : driver.CountryCode
                };

                return await App.jsonWebApiAgent.SendPosAsyncRequest<int>($"/api/Driver", account);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static async Task<List<OpenOrderParameter>> GetOpenOrders(int lastSyncId)
        {
            try
            {
                var res = await App.jsonWebApiAgent.SendGetAllAsyncRequest<OpenOrderParameter>($"/api/Order/OpenOrders/LastSyncId/{lastSyncId}");
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static async Task<List<OpenOrderParameter>> GetClosedOrders(int lastSyncId)
        {
            try
            {
                var res = await App.jsonWebApiAgent.SendGetAllAsyncRequest<OpenOrderParameter>($"/api/Order/ClosedOrders/LastSyncId/{lastSyncId}");
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static async Task<List<OrderParameter>> GetAllOrders()
        {
            try
            {
                var res = await App.jsonWebApiAgent.SendGetAllAsyncRequest<OrderParameter>($"/api/Order/GetOrdersByDriverId/{Globals.LoggedDriverId}");
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<int> UpdateOrderDocuments(string orderId, int type, OrderDocuments docs)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/OrderDocuments/OrderId/{orderId}/Type/{type}", docs);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static async Task<int> UpdateOrderSetDriverId(string driverId, string orderId, string Lon, string Lat)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Order/DriverId/{driverId}/OrderId/{orderId}/Lon/{Lon}/Lat/{Lat}",null);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<int> AcceptAddressChange(string orderId, string driverId)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Order/Accept/OrderId/{orderId}/Driverid/{driverId}", null);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<GoogleDirectionsResponse> GetMapsDirection(Position startLocation, Position endLocation, List<Position> waypointLocations)
        {
            try
            {
                //List<string> positionStrings = new List<string>();
                //foreach (var position in waypointLocations)
                //{
                //    positionStrings.Add($"{position.Latitude},{position.Longitude}");
                //}
                //var waypoints = string.Join("|", positionStrings);
                //var googleDirection = await App.jsonWebApiAgent.SendGetAsyncRequestMaps<GoogleDirectionsResponse>($"api/directions/json?mode=driving&transit_routing_preference=less_driving&origin={startLocation.Latitude},{startLocation.Longitude}&destination={endLocation.Latitude},{endLocation.Longitude}&waypoints={waypoints}&key={App.GOOGLE_MAP_API_KEY}");
                var googleDirection = await App.jsonWebApiAgent.SendGetAsyncRequestMaps<GoogleDirectionsResponse>($"api/directions/json?mode=driving&transit_routing_preference=less_driving&origin={startLocation.Latitude},{startLocation.Longitude}&destination={endLocation.Latitude},{endLocation.Longitude}&key={App.GOOGLE_MAP_API_KEY}");
                return googleDirection;
            }
            catch (Exception e)
            {
                return new GoogleDirectionsResponse();
            }
        }
        public static async Task<int> UpdateOrderLocationByDriverId(string driverId, string orderId, string Lon, string Lat)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Order/Location/DriverId/{driverId}/OrderId/{orderId}/Lon/{Lon}/Lat/{Lat}", null);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<int> UpdateOrderStatus(string orderId, string orderStatus)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Order/OrderId/{orderId}/Status/{orderStatus}", null);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<int> UpdateAccount(string driverId, Driver driver)
        {
            try
            {
                var account = new
                {
                    id = driver.Id,
                    driverId = driver.DriverId,
                    fullName = string.IsNullOrEmpty(driver.FullName) ? "" : driver.FullName,
                    username = string.IsNullOrEmpty(driver.Username) ? "" : driver.Username,
                    email = string.IsNullOrEmpty(driver.Email) ? "" : driver.Email,
                    password = string.IsNullOrEmpty(driver.Password) ? "" : driver.Password,
                    phone = string.IsNullOrEmpty(driver.Phone) ? "" : driver.Phone,
                    dateOfBirth = "2023-07-25T07:53:40.399Z",
                    gender = string.IsNullOrEmpty(driver.Gender) ? "" : driver.Gender,
                    accountPreferences = string.IsNullOrEmpty(driver.AccountPreferences) ? "" : driver.AccountPreferences,
                    termsAndCondition = driver.TermsAndCondition,
                    image = string.IsNullOrEmpty(driver.Image) ? "" : driver.Image,
                    country = string.IsNullOrEmpty(driver.Country) ? "Fiji" : driver.Country,
                    countryCode = string.IsNullOrEmpty(driver.CountryCode) ? "679" : driver.CountryCode,
                    referralCode = driver.ReferralCode
                };

                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Driver/{driverId}", account);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<List<OrderParameter>> GetOngoingOrdersByDriverId(string driverId)
        {
            try
            {
                var res = await App.jsonWebApiAgent.SendGetAllAsyncRequest<OrderParameter>($"/api/Order/OngoingOrder/DriverId/{driverId}");
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<int> UpsertVehicle(Vehicle vehicle)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPosAsyncRequest<int>($"/api/Vehicle", vehicle);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static async Task<Vehicle> GetVehicleByDriverId(string driverId)
        {
            try
            {
                return await App.jsonWebApiAgent.SendGetAsyncRequest<Vehicle>($"/api/Vehicle/DriverId/{driverId}");
            }
            catch (Exception e)
            {
                return new Vehicle();
            }
        }
        public static async Task<Driver> CheckUserInfoByEmail(string email)
        {
            try
            {
                return await App.jsonWebApiAgent.SendGetAsyncRequest<Driver>($"/api/Driver/Email/{email}");
            }
            catch (Exception e)
            {
                return new Driver();
            }
        }
        public static async Task<CustomerDetails> CheckUserInfoByCustomerId(string customerId)
        {
            try
            {
                return await App.jsonWebApiAgent.SendGetAsyncRequest<CustomerDetails>($"/api/Customer/Details/CustomerId/{customerId}");
            }
            catch (Exception e)
            {
                return new CustomerDetails();
            }
        }
        public static async Task<int> ArchiveOrder(string orderId)
        {
            try
            {
                return await App.jsonWebApiAgent.SendPutAsyncRequest<int>($"/api/Order/ArchiveOrder/Type/Driver/OrderId/{orderId}", null);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
