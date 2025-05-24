using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public enum RouteDirection { Incoming, Outgoing }

public class Route
{
    public int Id { get; set; }
    public string Location1 { get; set; }
    public string Location2 { get; set; }
    public RouteDirection Direction { get; set; }

    public ICollection<Truck> Trucks { get; set; }
    public ICollection<Shipment> Shipments { get; set; }
}

