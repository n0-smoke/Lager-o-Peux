using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int LoadCapacity { get; set; }
        public string Destination { get; set; }
        public string TruckId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, In Transit, Delivered
        public DateTime ScheduledDate { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public User? AssignedEmployee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

