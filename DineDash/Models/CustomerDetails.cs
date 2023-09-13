using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class CustomerDetails
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        public string Address { get; set; }
    }
}
