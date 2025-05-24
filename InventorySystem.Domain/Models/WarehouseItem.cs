using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public class WarehouseItem
{
    public int Id { get; set; }

    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }

    public int Amount { get; set; }
}

