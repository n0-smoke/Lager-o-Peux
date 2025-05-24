using System;
using System.Collections.Generic;

namespace InventorySystem.Domain.Models
{
    public class Truck
    {
        public int Id { get; set; }

        public string Identifier { get; set; } // e.g., "TRK-348"
        public int MaxCapacityKg { get; set; } // e.g., 10000

        public ICollection<Shipment> Shipments { get; set; }

        // New fields for maintenance tracking
        public bool IsUnderMaintenance { get; set; } = false;

        public string Status { get; set; } = "Available"; // e.g., "Available", "Unavailable"

        public DateTime LastMaintenanceDate { get; set; } = DateTime.UtcNow;

        public ICollection<TruckMaintenanceRecord> MaintenanceRecords { get; set; }
    }
}
