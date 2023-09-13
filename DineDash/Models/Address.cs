using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class Address
    {
        [PrimaryKey, JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("addressId")]
        public string AddressId { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("postCode")]
        public string PostCode { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("lon")]
        public double Lon { get; set; }
        [JsonProperty("lat")]
        public double Lat { get; set; }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (!String.IsNullOrEmpty(Address1)) builder.AppendLine(Address1);
            if (!String.IsNullOrEmpty(Street)) builder.Append(Street + " ");
            if (!String.IsNullOrEmpty(City)) builder.Append(City + " ");
            if (!String.IsNullOrEmpty(Country)) builder.Append(Country + " ");

            if (builder[builder.Length - 1] == ' ') builder.Remove(builder.Length - 1, 1);
            builder.AppendLine();
            if (builder[builder.Length - 1] == '\n') builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}
