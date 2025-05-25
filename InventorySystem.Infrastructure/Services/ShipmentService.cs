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
            Route? route = await _context.Routes.FirstOrDefaultAsync(r =>
                (r.Location1 == currentUserLocation && r.Location2 == destinationLocation) ||
                (r.Location2 == currentUserLocation && r.Location1 == destinationLocation));

            if (route == null)
                throw new InvalidOperationException("No route found between locations");

            // Step 3: Get selected truck
            var truck = await _context.Trucks
                .Include(t => t.Shipments)
                    .ThenInclude(s => s.ShipmentItems)
                .FirstOrDefaultAsync(t =>
                    t.Id == dto.TruckId &&
                    t.Availability == true &&
                    t.Location == currentUserLocation &&
                    t.RouteId == route.Id);

            if (truck == null)
                throw new InvalidOperationException("Invalid or unavailable truck");

            // Step 4: Calculate existing shipment weight on this truck
            var existingWeight = truck.Shipments
                .SelectMany(s => s.ShipmentItems)
                .Join(_context.InventoryItems,
                      si => si.InventoryItemId,
                      ii => ii.Id,
                      (si, ii) => si.Amount * ii.WeightPerUnit)
                .Sum();

            // Step 5: Calculate weight of this new shipment
            double newShipmentWeight = 0;
            foreach (var item in dto.Items)
            {
                var inventoryItem = await _context.InventoryItems.FindAsync(item.InventoryItemId);
                if (inventoryItem == null)
                    throw new InvalidOperationException($"Invalid inventory item ID: {item.InventoryItemId}");

                newShipmentWeight += item.Amount * inventoryItem.WeightPerUnit;
            }

            double totalProjectedWeight = existingWeight + newShipmentWeight;

            if (totalProjectedWeight > truck.LoadCapacity)
                throw new InvalidOperationException($"This shipment would exceed the truck’s capacity of {truck.LoadCapacity} kg. Current: {existingWeight}, New: {newShipmentWeight}");

            // Step 6: Create and persist the shipment
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

        public async Task UpdateShipmentAsync(int shipmentId, ShipmentCreateDto dto, string currentUserLocation)
        {
            var existing = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (existing == null)
                throw new InvalidOperationException("Shipment not found.");

            _context.ShipmentItems.RemoveRange(existing.ShipmentItems);

            // Determine destination location
            string destinationLocation = dto.DestinationType.ToLower() switch
            {
                "client" => _context.Clients.FirstOrDefault(c => c.Id == dto.DestinationEntityId)?.Location,
                "warehouse" => _context.Warehouses.FirstOrDefault(w => w.Id == dto.DestinationEntityId)?.Location,
                _ => throw new ArgumentException("Invalid destination type")
            };

            if (string.IsNullOrEmpty(destinationLocation))
                throw new InvalidOperationException("Could not resolve destination location");

            // Get route based on resolved location
            var route = _context.Routes.FirstOrDefault(r =>
                (r.Location1 == currentUserLocation && r.Location2 == destinationLocation) ||
                (r.Location2 == currentUserLocation && r.Location1 == destinationLocation));

            if (route == null)
                throw new InvalidOperationException("No valid route found.");

            existing.TruckId = dto.TruckId;
            existing.RouteId = route.Id;

            existing.ShipmentItems = dto.Items.Select(i => new ShipmentItem
            {
                InventoryItemId = i.InventoryItemId,
                Amount = i.Amount
            }).ToList();

            await _context.SaveChangesAsync();
        }



    }
}
