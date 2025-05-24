using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Application.DTOs;

public class ShipmentCreateDto
{
    public int DestinationEntityId { get; set; }
    public string DestinationType { get; set; } // "Client" or "Warehouse"

    public List<ShipmentItemCreateDto> Items { get; set; }

    public int TruckId { get; set; } // Chosen after filtering
}

