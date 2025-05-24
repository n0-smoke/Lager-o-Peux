using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Domain.Models;

public class Truck
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int RouteId { get; set; }
    public Route Route { get; set; }

    public int DriverId { get; set; } // FK to Employee table

    public double LoadCapacity { get; set; }
    public string Location { get; set; }
    public double FuelConsumption { get; set; }
    public double Mileage { get; set; }
    public bool Availability { get; set; }

    public ICollection<Shipment> Shipments { get; set; }
}
