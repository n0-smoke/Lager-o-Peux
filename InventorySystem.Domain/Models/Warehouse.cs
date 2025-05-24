using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public double Capacity { get; set; }

    public ICollection<WarehouseItem> WarehouseItems { get; set; }
}
