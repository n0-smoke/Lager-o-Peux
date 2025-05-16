using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string Supplier { get; set; }
        public int ReorderThreshold { get; set; } = 10;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double WeightPerUnit { get; set; } // in kg

        // public ICollection<InventoryTransaction>? Transactions { get; set; }
    }
}

