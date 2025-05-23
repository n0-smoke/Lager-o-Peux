using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventorySystem.Domain.Models
{

    public class Truck
    {
        public int TruckId { get; set; }
        public string LicensePlate { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }

        public bool IsUnderMaintenance { get; set; }

 
        public List<TruckMaintenanceRecord> MaintenanceRecords { get; set; }

        public string Status { get; set; } = "Available";

        public DateTime LastMaintenanceDate { get; set; } = DateTime.UtcNow;
        
    }
}
