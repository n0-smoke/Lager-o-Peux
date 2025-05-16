using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class ShipmentInventoryItem
    {
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }

        public double TotalWeightKg =>
            InventoryItem != null ? InventoryItem.WeightPerUnit * Quantity : 0;
    }
}

