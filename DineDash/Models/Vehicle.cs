using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class Vehicle
    {
        [JsonProperty("id"),PrimaryKey]
        public int Id { get; set; }
        [JsonProperty("driverId")]
        public string DriverId { get; set; }
        [JsonProperty("carDescription")]
        public string CarDescription { get; set; }
        [JsonProperty("carRegistration")]
        public string CarRegistration { get; set; }
        [JsonProperty("driversPhoto")]
        public string DriversPhoto { get; set; }
        [JsonProperty("carPhoto")]
        public string CarPhoto { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updateAt")]
        public DateTime? UpdateAt { get; set; }
    }
}
