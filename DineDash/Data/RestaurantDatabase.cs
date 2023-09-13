using DineDash.Models;
using GoogleApi.Entities.Search.Common;
using GoogleApi.Entities.Search.Video.Common;
using GoogleApi.Entities.Search.Video.Common.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Order = DineDash.Models.Order;

namespace DineDash.Data
{
    public class RestaurantDatabase : IRestaurantDatabase
    {
        static SQLiteAsyncConnection Database;
        public RestaurantDatabase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RestaurantDriver.db3");
            Database = new SQLiteAsyncConnection(dbPath);
            Task.Run(async () =>
            {
                await Database.CreateTableAsync<Driver>();
                await Database.CreateTableAsync<Order>();
                await Database.CreateTableAsync<OrderItem>();
                await Database.CreateTableAsync<OpenOrder>();
                await Database.CreateTableAsync<OpenOrderItem>();
                await Database.CreateTableAsync<HasNewOpenOrders>();
                await Database.CreateTableAsync<Vehicle>();
            });
        }

        public async Task<int> InsertOpenOrder(List<OpenOrder> items)
        {
            int count = 0;
            foreach (var newItem in items)
            {
                var existingItem = await App.RestaurantDatabase.GetOpenOrderById(newItem.Id);
                if (existingItem?.OrderId == null)
                {
                    var res = await Database.InsertOrReplaceAsync(newItem);
                    count++;
                }
            }
            var getHasNewOrderItems = await GetHasNewOpenOrders(Globals.LoggedDriverId);
            int maxId = items.Max(item => item.Id);
            if (getHasNewOrderItems == null)
            {
                getHasNewOrderItems = new HasNewOpenOrders
                {
                    Id = 0,
                    DriverId = Globals.LoggedDriverId,
                    HasNewItems = count > 0 ? true : false,
                    lastSyncId = maxId
                };
            }
            else
            {
                getHasNewOrderItems.lastSyncId = maxId;
                getHasNewOrderItems.HasNewItems = count > 0 ? true : false;
            }
            await InsertOrUpdateHasNewOpenOrders(getHasNewOrderItems);
            
            return 1;
        }
        public async Task<int> DeleteOpenOrderAsync(List<int> items)
        {
            foreach (var id in items)
            {
                var recordToDelete = await Database.Table<OpenOrder>().Where(x => x.Id == id).FirstOrDefaultAsync();
                if (recordToDelete != null)
                {
                    await Database.DeleteAsync(recordToDelete);
                }
            }
             return 1;
        }

        public async Task<int> AddAllOpenOrderItem(List<OpenOrderItem> items)
        {
            await Database.DeleteAllAsync<OpenOrderItem>();
            return await Database.InsertAllAsync(items);
        }

        public async Task<OpenOrderParameter> GetOpenOrderById(int id)
        {
            var item = await Database.Table<OpenOrder>().Where(i => i.Id == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            OpenOrderParameter orderParameter;
            if (item?.OrderId != null)
            {
                orderParameter = new OpenOrderParameter
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.OrderId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Discount = item.Discount,
                    Total = item.Total,
                    Status = item.Status,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    OrderStatus = null,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    OnTheWayTime = item.OnTheWayTime,
                    ForPickUpTime = item.ForPickUpTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    OrderItems = await Database.Table<OpenOrderItem>().Where(i => i.OrderId == item.OrderId).OrderByDescending(x => x.Id).ToListAsync(),
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                    IsNotViewed = item.IsNotViewed
                };
                return orderParameter;
            }
            return orderParameter = new OpenOrderParameter();
        }
        public async Task<List<OpenOrderParameter>> GetOpenOrders()
        {
            List<OpenOrderParameter> orders = new List<OpenOrderParameter>();
            var res = await Database.Table<OpenOrder>().OrderByDescending(x => x.Id).ToListAsync();
            foreach (var item in res)
            {
                OpenOrderParameter orderParameter = new OpenOrderParameter
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.OrderId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Total = item.Total,
                    Discount = item.Discount,
                    Status = item.Status,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    OrderStatus = null,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    OnTheWayTime = item.OnTheWayTime,
                    ForPickUpTime = item.ForPickUpTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    OrderItems = await Database.Table<OpenOrderItem>().Where(i => i.OrderId == item.OrderId).OrderByDescending(x => x.Id).ToListAsync(),
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                    IsNotViewed = item.IsNotViewed
                };
                orders.Add(orderParameter);
            }
            return orders;
        }
        public async Task<int> AddAllOrder(List<Order> items)
        {
            await Database.DeleteAllAsync<Order>();
            return await Database.InsertAllAsync(items);
        }

        public async Task<int> AddAllOrderItem(List<OrderItem> items)
        {
            await Database.DeleteAllAsync<OrderItem>();
            return await Database.InsertAllAsync(items);
        }

        public async Task<Driver> GetDriverAsync(string id)
        {
            var res = await Database.Table<Driver>().Where(x => x.DriverId == id).ToListAsync();
            if (res.Count() == 0)
            {
                return new Driver();
            }
            return await Database.Table<Driver>().Where(x => x.DriverId == id).FirstAsync();
        }

        public async Task<Driver> GetDriverAsync(string email, string password)
        {
            var res = await Database.Table<Driver>().Where(x => x.Email == email && x.Password == password).ToListAsync();
            if (res.Count() == 0)
            {
                return new Driver();
            }
            return await Database.Table<Driver>().Where(x => x.Email == email && x.Password == password).FirstAsync();
        }

        public async Task<OrderParameter> GetOrderByOrderId(string orderId)
        {
            var item = await Database.Table<Order>().Where(i => i.OrderId == orderId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            OrderParameter orderParameter;
            if (item?.OrderId != null)
            {
                orderParameter = new OrderParameter
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.OrderId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Discount = item.Discount,
                    Total = item.Total,
                    Status = item.Status,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    OrderStatus = null,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    OnTheWayTime = item.OnTheWayTime,
                    ForPickUpTime = item.ForPickUpTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    OrderItems = await Database.Table<OrderItem>().Where(i => i.OrderId == item.OrderId).OrderByDescending(x => x.Id).ToListAsync(),
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                    IsDriverArchive = item.IsDriverArchive
                };
                return orderParameter;
            }
            return orderParameter = new OrderParameter();
        }

        public async Task<List<OrderParameter>> GetOrders()
        {
            List<OrderParameter> orders = new List<OrderParameter>();
            var res = await Database.Table<Order>().OrderByDescending(x => x.Id).ToListAsync();
            foreach (var item in res)
            {
                OrderParameter orderParameter = new OrderParameter
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.OrderId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Total = item.Total,
                    Discount = item.Discount,
                    Status = item.Status,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    OrderStatus = null,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    OnTheWayTime = item.OnTheWayTime,
                    ForPickUpTime = item.ForPickUpTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    OrderItems = await Database.Table<OrderItem>().Where(i => i.OrderId == item.OrderId).OrderByDescending(x => x.Id).ToListAsync(),
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                    IsDriverArchive = item.IsDriverArchive
                };
                orders.Add(orderParameter);
            }
            return orders;
        }

        public async Task<List<OrderParameter>> GetOrdersByDriverId(string driverId)
        {
            List<OrderParameter> orders = new List<OrderParameter>();
            var res = await Database.Table<Order>().Where(i=>i.DriverId == driverId).OrderByDescending(x => x.Id).ToListAsync();
            foreach (var item in res)
            {
                OrderParameter orderParameter = new OrderParameter
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    CustomerId = item.OrderId,
                    DateGmt = item.DateGmt,
                    Address = item.Address,
                    AddressTitle = item.AddressTitle,
                    Shipping = item.Shipping,
                    Total = item.Total,
                    Discount = item.Discount,
                    Status = item.Status,
                    ModeOfPayment = item.ModeOfPayment,
                    IsOngoingOrder = item.IsOngoingOrder,
                    OrderStatus = null,
                    PlacedTime = item.PlacedTime,
                    ProcessingTime = item.ProcessingTime,
                    OnTheWayTime = item.OnTheWayTime,
                    ForPickUpTime = item.ForPickUpTime,
                    DeliveredTime = item.DeliveredTime,
                    CanceledTime = item.CanceledTime,
                    GrandTotal = item.GrandTotal,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    DriverId = item.DriverId,
                    DriverLat = item.DriverLat,
                    DriverLon = item.DriverLon,
                    OrderItems = await Database.Table<OrderItem>().Where(i => i.OrderId == item.OrderId).OrderByDescending(x => x.Id).ToListAsync(),
                    IsChangeAddress = item.IsChangeAddress,
                    ChangeAddress = item.ChangeAddress,
                    ChangeAddressTitle = item.ChangeAddressTitle,
                    ChangeAddressLat = item.ChangeAddressLat,
                    ChangeAddressLon = item.ChangeAddressLon,
                    AdditionalFee = item.AdditionalFee,
                    IsDriverArchive = item.IsDriverArchive
                };
                orders.Add(orderParameter);
            }
            return orders;
        }

        public async Task IsTableExists(string tableName)
        {
            try
            {
                await Task.Delay(0);
                var tableInfo = Database.GetConnection().GetTableInfo(tableName);
                if (tableInfo.Count > 0)
                {
                    Console.WriteLine("exists");
                }
                else
                {
                    Console.WriteLine("not exists");
                }
            }
            catch
            {
                Console.WriteLine("not exists");
            }
        }

        public async Task<bool> UpdateDriverAsync(Driver customer)
        {
            var count = await Database.Table<Driver>().Where(x => x.DriverId == customer.DriverId).ToListAsync();
            if (count.Count() == 0)
            {
                var res = await Database.InsertAsync(customer);
                return res == 1 ? true : false;
            }
            var result = await Database.InsertOrReplaceAsync(customer);
            return result == 1 ? true : false;
        }

        public async Task<int> InsertOrUpdateHasNewOpenOrders(HasNewOpenOrders items)
        {
            int maxId = Database.Table<HasNewOpenOrders>().OrderByDescending(item => item.Id).FirstOrDefaultAsync()?.Id ?? 0;
            var count = await Database.Table<HasNewOpenOrders>().Where(x => x.DriverId == items.DriverId).ToListAsync();
            if (count.Count() == 0)
            {
                items.Id = maxId + 1;
                var res = await Database.InsertAsync(items);
                return res;
            }
            var result = await Database.UpdateAsync(items);
            return result;
        }

        public async Task<HasNewOpenOrders> GetHasNewOpenOrders(string driverId)
        {
            try
            {
                return await Database.Table<HasNewOpenOrders>().Where(x => x.DriverId == driverId).FirstOrDefaultAsync();
            }
            catch(Exception )
            {
            }
            return null;
        }

        public async Task<int> UpdateIsViewedOpenOrders(int id)
        {
            var itemToUpdate = await Database.Table<OpenOrder>().FirstOrDefaultAsync(item => item.Id == id);
            if (itemToUpdate != null)
            {
                itemToUpdate.IsNotViewed = false;
                return await Database.UpdateAsync(itemToUpdate);
            }
            return 0;
        }

        public async Task<int> DeleteOpenOrderAsyncById(int id)
        {
            var itemToUpdate = await Database.Table<OpenOrder>().FirstOrDefaultAsync(item => item.Id == id);
            if (itemToUpdate != null)
            {
                return await Database.DeleteAsync(itemToUpdate);
            }
            return 0;
        }

        public async Task<int> ClearAllTables()
        {
            await Database.DeleteAllAsync<Driver>();
            await Database.DeleteAllAsync<Order>();
            await Database.DeleteAllAsync<OrderItem>();
            await Database.DeleteAllAsync<OpenOrder>();
            await Database.DeleteAllAsync<OpenOrderItem>();
            await Database.DeleteAllAsync<HasNewOpenOrders>();
            await Database.DeleteAllAsync<Vehicle>();
            return 1;
        }

        public async Task<Vehicle> GetVehicleAsync(string driverId)
        {
            var res = await Database.Table<Vehicle>().Where(x => x.DriverId == driverId).ToListAsync();
            if (res.Count() == 0)
            {
                return new Vehicle();
            }
            return await Database.Table<Vehicle>().Where(x => x.DriverId == driverId).FirstAsync();
        }

        public async Task<bool> UpsertVehicle(Vehicle vehicle)
        {
            var count = await Database.Table<Driver>().Where(x => x.DriverId == vehicle.DriverId).ToListAsync();
            if (count.Count() == 0)
            {
                var res = await Database.InsertAsync(vehicle);
                return res == 1 ? true : false;
            }
            var result = await Database.InsertOrReplaceAsync(vehicle);
            return result == 1 ? true : false;
        }

        public async Task<int> DeleteOpenOrderNotInOrderList(List<OpenOrder> item)
        {
            try
            {
                List<OpenOrder> fetchedOrderIds = await Database.Table<OpenOrder>().ToListAsync();
                List<int> orderIdsToKeep = new List<int>();
                foreach (var id in item)
                {
                    orderIdsToKeep.Add(id.Id);
                }
                var orderIdToDelete = await Database.Table<OpenOrder>()
                    .Where(order => !orderIdsToKeep.Contains(order.Id))
                    .ToListAsync();
                foreach (var orderId in orderIdToDelete)
                {
                    await Database.DeleteAsync<OpenOrder>(orderId.Id);
                }
                return 1;
            }
            catch(Exception e)
            {

            }
            return 0;
        }
    }
}
