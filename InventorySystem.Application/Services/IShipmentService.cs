using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Services;

public interface IShipmentService
{
    Task CreateShipmentAsync(ShipmentCreateDto dto, string currentUserLocation);
}

