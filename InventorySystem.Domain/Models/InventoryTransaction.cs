using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public InventoryItem? Item { get; set; }
        public int QuantityChange { get; set; }
        public string TransactionType { get; set; } // Restock, Sale, etc.
        public int? PerformedByUserId { get; set; }
        public User? PerformedByUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

