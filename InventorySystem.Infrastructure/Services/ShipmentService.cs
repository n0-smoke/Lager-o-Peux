using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Application.DTOs;
using InventorySystem.Application.Services;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _context;

        public ShipmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateShipmentAsync(ShipmentCreateDto dto, string currentUserLocation)
        {
            // Step 1: Resolve destination location
            string destinationLocation = dto.DestinationType.ToLower() switch
            {
                "client" => (await _context.Clients.FindAsync(dto.DestinationEntityId))?.Location,
                "warehouse" => (await _context.Warehouses.FindAsync(dto.DestinationEntityId))?.Location,
                _ => throw new ArgumentException("Invalid destination type")
            };

            if (string.IsNullOrEmpty(destinationLocation))
                throw new InvalidOperationException("Could not resolve destination location");

            // Step 2: Determine direction and route
            RouteDirection direction;
            Route? route;

            if ((route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Location1 == currentUserLocation && r.Location2 == destinationLocation)) != null)
            {
                direction = RouteDirection.Outgoing;
            }
            else if ((route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Location2 == currentUserLocation && r.Location1 == destinationLocation)) != null)
            {
                direction = RouteDirection.Incoming;
            }
            else
            {
                throw new InvalidOperationException("No route found between locations");
            }

            // Step 3: Get selected truck
            var truck = await _context.Trucks
                .Include(t => t.Shipments)
                .FirstOrDefaultAsync(t =>
                    t.Id == dto.TruckId &&
                    t.Availability == true &&
                    t.Location == currentUserLocation &&
                    t.RouteId == route.Id);

            if (truck == null)
                throw new InvalidOperationException("Invalid or unavailable truck");

            // Step 4: Calculate total weight
            double totalWeight = 0;

            foreach (var item in dto.Items)
            {
                var inventoryItem = await _context.InventoryItems.FindAsync(item.InventoryItemId);
                if (inventoryItem == null)
                    throw new InvalidOperationException($"Invalid inventory item ID: {item.InventoryItemId}");

                totalWeight += item.Amount * inventoryItem.WeightPerUnit;
            }

            if (totalWeight > truck.LoadCapacity)
                throw new InvalidOperationException("Total shipment weight exceeds truck capacity");

            // Step 5: Create Shipment entity
            var shipment = new Shipment
            {
                TruckId = truck.Id,
                RouteId = route.Id,
                ShipmentItems = dto.Items.Select(i => new ShipmentItem
                {
                    InventoryItemId = i.InventoryItemId,
                    Amount = i.Amount
                }).ToList()
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
        }
    }
}
