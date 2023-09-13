using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Models
{
    public class HasNewOpenOrders
    {
        public int Id { get; set; }
        [PrimaryKey]
        public string DriverId { get; set; }
        public int lastSyncId { get; set; }
        public bool HasNewItems { get; set; }
    }
}
