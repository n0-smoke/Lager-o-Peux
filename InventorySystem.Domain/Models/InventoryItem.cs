using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double WeightPerUnit { get; set; }

    public ICollection<WarehouseItem> WarehouseItems { get; set; }
    public ICollection<ShipmentItem> ShipmentItems { get; set; }
}

