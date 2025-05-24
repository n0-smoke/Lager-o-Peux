using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Application.DTOs;

public class ShipmentItemCreateDto
{
    public int InventoryItemId { get; set; }
    public int Amount { get; set; }
}

