using DineDash.Enums;
using DineDash.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class OrderParameter : BaseViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("dateGmt")]
        public DateTime DateGmt { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("addressTitle")]
        public string AddressTitle { get; set; }
        [JsonProperty("shipping")]
        public double Shipping { get; set; }
        [JsonProperty("discount")]
        public double Discount { get; set; }
        [JsonProperty("total")]
        public double Total { get; set; }
        [JsonProperty("modeOfPayment")]
        public int ModeOfPayment { get; set; }
        [JsonProperty("isOngoingOrder")]
        public bool IsOngoingOrder { get; set; }
        [JsonProperty("status")]
        public string Status 
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            } 
        }
        public OrderStatus? OrderStatus { get; set; }

        [JsonProperty("placedTime")]
        public DateTime? PlacedTime { get; set; }

        [JsonProperty("processingTime")]

        public DateTime? ProcessingTime { get; set; }

        [JsonProperty("onTheWayTime")]

        public DateTime? OnTheWayTime { get; set; }

        [JsonProperty("forPickUpTime")]
        public DateTime? ForPickUpTime { get; set; }

        [JsonProperty("deliveredTime")]

        public DateTime? DeliveredTime { get; set; }

        [JsonProperty("canceledTime")]

        public DateTime? CanceledTime { get; set; }

        [JsonProperty("grandTotal")]
        public double GrandTotal { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }

        [JsonProperty("orderItems")]
        public List<OrderItem> OrderItems { get; set; }

        [JsonProperty("driverId")]
        public string DriverId { get; set; }

        [JsonProperty("isChangeAddress")]
        public bool IsChangeAddress { get; set; }
        [JsonProperty("changeAddress")]
        public string ChangeAddress { get; set; }
        [JsonProperty("changeAddressTitle")]
        public string ChangeAddressTitle { get; set; }
        [JsonProperty("changeAddressLat")]
        public string ChangeAddressLat { get; set; }
        [JsonProperty("changeAddressLon")]
        public string ChangeAddressLon { get; set; }
        [JsonProperty("additionalFee")]
        public double AdditionalFee { get; set; }
        [JsonProperty("isChangeAddressAccepted")]
        public bool IsChangeAddressAccepted { get; set; }

        [JsonProperty("driverLat")]
        public string DriverLat { 
            get => _driverLat;
            set
            {
                _driverLat = value;
                OnPropertyChanged("DriverLat");
            }
        }

        [JsonProperty("driverLon")]
        public string DriverLon 
        {
            get => _driverLon;
            set
            {
                _driverLon = value;
                OnPropertyChanged("DriverLon");
            }
        }
        [JsonProperty("isDriverArchive")]
        public bool IsDriverArchive
        {
            get => isDriverArchive;
            set
            {
                isDriverArchive = value;
                OnPropertyChanged("IsDriverArchive");
            }
        }
        bool isExpanded = false;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        string address2Display;
        public string Address2Display
        {
            get => address2Display;
            set
            {
                address2Display = value;
                OnPropertyChanged("Address2Display");
            }
        }
        string _driverLat = "";
        string _driverLon = "";
        bool isDriverArchive = false;
        string _status = "Placed";
    }

    public class OpenOrderParameter : BaseViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("dateGmt")]
        public DateTime DateGmt { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("addressTitle")]
        public string AddressTitle { get; set; }
        [JsonProperty("shipping")]
        public double Shipping { get; set; }
        [JsonProperty("discount")]
        public double Discount { get; set; }
        [JsonProperty("total")]
        public double Total { get; set; }
        [JsonProperty("modeOfPayment")]
        public int ModeOfPayment { get; set; }
        [JsonProperty("isOngoingOrder")]
        public bool IsOngoingOrder { get; set; }
        [JsonProperty("status")]
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        public OrderStatus? OrderStatus { get; set; }

        [JsonProperty("placedTime")]
        public DateTime? PlacedTime { get; set; }

        [JsonProperty("processingTime")]

        public DateTime? ProcessingTime { get; set; }

        [JsonProperty("onTheWayTime")]

        public DateTime? OnTheWayTime { get; set; }

        [JsonProperty("forPickUpTime")]
        public DateTime? ForPickUpTime { get; set; }

        [JsonProperty("deliveredTime")]

        public DateTime? DeliveredTime { get; set; }

        [JsonProperty("canceledTime")]

        public DateTime? CanceledTime { get; set; }

        [JsonProperty("grandTotal")]
        public double GrandTotal { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }

        [JsonProperty("orderItems")]
        public List<OpenOrderItem> OrderItems { get; set; }

        [JsonProperty("driverId")]
        public string DriverId { get; set; }

        [JsonProperty("isChangeAddress")]
        public bool IsChangeAddress { get; set; }
        [JsonProperty("changeAddress")]
        public string ChangeAddress { get; set; }
        [JsonProperty("changeAddressTitle")]
        public string ChangeAddressTitle { get; set; }
        [JsonProperty("changeAddressLat")]
        public string ChangeAddressLat { get; set; }
        [JsonProperty("changeAddressLon")]
        public string ChangeAddressLon { get; set; }
        [JsonProperty("additionalFee")]
        public double AdditionalFee { get; set; }
        [JsonProperty("isChangeAddressAccepted")]
        public bool IsChangeAddressAccepted { get; set; }

        [JsonProperty("driverLat")]
        public string DriverLat
        {
            get => _driverLat;
            set
            {
                _driverLat = value;
                OnPropertyChanged("DriverLat");
            }
        }

        [JsonProperty("driverLon")]
        public string DriverLon
        {
            get => _driverLon;
            set
            {
                _driverLon = value;
                OnPropertyChanged("DriverLon");
            }
        }
        bool isExpanded = false;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        string _driverLat = "";
        string _driverLon = "";
        string _status = "Placed";
        bool isNotViewed = true;
        public bool IsNotViewed
        {
            get => isNotViewed;
            set
            {
                isNotViewed = value;
                OnPropertyChanged("IsNotViewed");
            }
        }
        string address2Display;
        public string Address2Display
        {
            get => address2Display;
            set
            {
                address2Display = value;
                OnPropertyChanged("Address2Display");
            }
        }
    }
}
