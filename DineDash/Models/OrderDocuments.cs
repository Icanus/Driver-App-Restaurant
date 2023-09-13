using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class OrderDocuments
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("onTheWayImage")]
        public string OnTheWayImage { get; set; }
        [JsonProperty("onTheWayImageTime")]
        public DateTime OnTheWayImageTime { get; set; }
        [JsonProperty("deliveredImage")]
        public string DeliveredImage { get; set; }
        [JsonProperty("deliveredImageTime")]
        public DateTime DeliveredImageTime { get; set; }
        [JsonProperty("cancelledImage")]
        public string CancelledImage { get; set; }
        [JsonProperty("cancelledImageTime")]
        public DateTime CancelledImageTime { get; set; }
        [JsonProperty("cancelledReason")]
        public string CancelledReason { get; set; }
    }
}
