using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public enum ShipmentDirection
    {
        Incoming,
        Outgoing
    }

    public class Shipment
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public ShipmentDirection Direction { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime ScheduledDate { get; set; }

        public int TruckId { get; set; }
        public Truck Truck { get; set; }

        public ICollection<ShipmentInventoryItem> ShipmentItems { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
