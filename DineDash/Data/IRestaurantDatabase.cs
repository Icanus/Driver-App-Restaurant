using DineDash.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DineDash.Data
{
    public interface IRestaurantDatabase
    {
        Task IsTableExists(string tableName);
        Task<Driver> GetDriverAsync(string id);
        Task<Driver> GetDriverAsync(string email, string password);
        Task<bool> UpdateDriverAsync(Driver customer);
        Task<int> AddAllOrder(List<Order> items);
        Task<int> AddAllOrderItem(List<OrderItem> items);
        Task<List<OrderParameter>> GetOrders();
        Task<List<OrderParameter>> GetOrdersByDriverId(string driverId);
        Task<OrderParameter> GetOrderByOrderId(string orderId);
        Task<List<OpenOrderParameter>> GetOpenOrders();
        Task<OpenOrderParameter> GetOpenOrderById(int id);
        Task<int> DeleteOpenOrderAsync(List<int> item);
        Task<int> DeleteOpenOrderNotInOrderList(List<OpenOrder> item);
        Task<int> DeleteOpenOrderAsyncById(int id);
        Task<int> InsertOpenOrder(List<OpenOrder> items);
        Task<int> AddAllOpenOrderItem(List<OpenOrderItem> items);
        Task<int> InsertOrUpdateHasNewOpenOrders(HasNewOpenOrders items);
        Task<HasNewOpenOrders> GetHasNewOpenOrders(string driverId);
        Task<int> UpdateIsViewedOpenOrders(int id);
        Task<int> ClearAllTables();
        Task<Vehicle> GetVehicleAsync(string driverId);
        Task<bool> UpsertVehicle(Vehicle vehicle);
    }
}
