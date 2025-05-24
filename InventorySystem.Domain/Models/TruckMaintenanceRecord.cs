using System;

namespace InventorySystem.Domain.Models
{
    public class TruckMaintenanceRecord
    {
        public int Id { get; set; }

        public int TruckId { get; set; }
        public Truck Truck { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;
    }

    public enum MaintenanceStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
