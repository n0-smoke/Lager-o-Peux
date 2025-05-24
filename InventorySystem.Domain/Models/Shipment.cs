using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public class Shipment
{
    public int Id { get; set; }

    public int TruckId { get; set; }
    public Truck Truck { get; set; }

    public int RouteId { get; set; }
    public Route Route { get; set; }

    public ICollection<ShipmentItem> ShipmentItems { get; set; }
}

