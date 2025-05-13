using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // public ICollection<TaskAssignment>? TasksAssigned { get; set; }
        // public ICollection<Shipment>? ShipmentsAssigned { get; set; }
        // public ICollection<InventoryTransaction>? TransactionsPerformed { get; set; }
    }
}
