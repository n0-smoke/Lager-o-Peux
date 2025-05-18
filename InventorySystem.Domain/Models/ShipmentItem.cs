using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class ShipmentItem
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }
    }
}
