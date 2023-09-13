using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using DineDash.Models;
using System.Linq;
using DineDash.ViewModels;
using Xamarin.Forms;

namespace DineDash.Services
{
    public class SignalRService : BaseViewModel
    {
        //private HubConnection _hubConnection { get; set; }

        //public SignalRService()
        //{
        //    _hubConnection = new HubConnectionBuilder()
        //        .WithUrl($"{App.APIUrl}/ChatHub")
        //        .WithAutomaticReconnect()
        //        .Build();

        //    _hubConnection.On<IEnumerable<OrderParameter>>("ReceiveOrderRecords", OnReceiveRecords);
        //    _hubConnection.On<IEnumerable<OrderParameter>>("ReceivedOpenOrders", OnReceivedOpenOrders);
        //    _hubConnection.Closed += async (exception) =>
        //    {
        //        // Connection closed event handler
        //        // Attempt to reconnect here
        //        await ReconnectAsync();
        //    };

        //    _hubConnection.Reconnecting += async (exception) =>
        //    {
        //        // Reconnecting event handler
        //        // You can update UI to show reconnection status
        //    };

        //    _hubConnection.Reconnected += async (connectionId) =>
        //    {
        //        // Reconnected event handler
        //        // Update UI to indicate reconnection success
        //    };
        //    Communication();
        //}

        //public async Task ReconnectAsync()
        //{
        //    while (_hubConnection.State != HubConnectionState.Connected)
        //    {
        //        try
        //        {
        //            await _hubConnection.StartAsync();
        //            break; // Exit the loop on successful reconnection
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle reconnection error, e.g., delay and retry
        //            await Task.Delay(1000); // Wait for a moment before retrying
        //        }
        //    }
        //}

        //public async Task FetchAndSendRecordsByDriverId(string driverId)
        //{
        //    try
        //    {
        //        await _hubConnection.InvokeAsync("FetchAndSendRecordsByDriverId", driverId);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_hubConnection.State == HubConnectionState.Disconnected)
        //        {
        //            await ReconnectAsync(); // Reconnect if the connection is not active
        //            await FetchAndSendRecordsByDriverId(driverId);
        //        }
        //            // Handle exceptions, e.g., log the error or display an error message
        //    }
        //}
        //public async Task FetchOpenOrders()
        //{
        //    try
        //    {
        //        await _hubConnection.InvokeAsync("FetchOpenOrders");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_hubConnection.State == HubConnectionState.Disconnected)
        //        {
        //            await ReconnectAsync(); // Reconnect if the connection is not active
        //            await FetchOpenOrders();
        //        }
        //        // Handle exceptions, e.g., log the error or display an error message
        //    }
        //}

        //void Communication()
        //{
        //    MessagingCenter.Unsubscribe<object>(this, "FetchAndSendRecordsByDriverId");
        //    MessagingCenter.Subscribe<object>(this, "FetchAndSendRecordsByDriverId", async(args) =>
        //    {
        //        await FetchAndSendRecordsByDriverId(Globals.LoggedDriverId);
        //    });
        //}

        //private void OnReceiveRecords(IEnumerable<OrderParameter> records)
        //{
        //    List <OrderParameter> Order;
        //    if (records.Count() > 0)
        //        Order = records.ToList();
        //    else
        //        Order = new List<OrderParameter>();
        //    //Globals.OngoingOrders = Order;
        //    MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders");
        //    MessagingCenter.Send<object, List<OrderParameter>>(this, "updateOngoingOrders", Order);
        //    MessagingCenter.Unsubscribe<object>(this, "updateOngoingOrders2");
        //    MessagingCenter.Send<object>(this, "updateOngoingOrders2");
        //}

        //private void OnReceivedOpenOrders(IEnumerable<OrderParameter> records)
        //{
        //    List<OrderParameter> Order;
        //    if (records.Count() > 0)
        //        Order = records.ToList();
        //    else
        //        Order = new List<OrderParameter>();

        //    MessagingCenter.Unsubscribe<object>(this, "GetOpenOrders");
        //    MessagingCenter.Send<object, List<OrderParameter>>(this, "GetOpenOrders", Order);
        //}
    }
}
