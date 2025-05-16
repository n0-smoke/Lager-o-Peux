using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Identifier { get; set; } // e.g., "TRK-348"
        public int MaxCapacityKg { get; set; } // e.g., 10000

        public ICollection<Shipment> Shipments { get; set; }
    }
}

