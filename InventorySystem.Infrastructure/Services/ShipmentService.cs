using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;

namespace InventorySystem.Infrastructure.Services
{
    public class ShipmentService
    {
        private readonly AppDbContext _context;

        public ShipmentService(AppDbContext context)
        {
            _context = context;
        }

        public List<Shipment> GetAllShipments()
        {
            return _context.Shipments.OrderByDescending(s => s.CreatedAt).ToList();
        }

        public void AddShipment(Shipment shipment)
        {
            // Handle shipment items separately to avoid tracking conflicts
            var shipmentItems = shipment.ShipmentItems?.ToList();
            shipment.ShipmentItems = null;
            
            // Add the shipment first
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
            
            // Then add the shipment items if they exist
            if (shipmentItems != null && shipmentItems.Any())
            {
                foreach (var item in shipmentItems)
                {
                    // Ensure the inventory item is not being tracked
                    if (item.InventoryItem != null)
                    {
                        var existingItem = _context.InventoryItems.Local
                            .FirstOrDefault(i => i.Id == item.InventoryItem.Id);
                            
                        if (existingItem != null)
                        {
                            _context.Entry(existingItem).State = EntityState.Detached;
                        }
                        
                        // Store the ID and set the reference to null
                        item.InventoryItemId = item.InventoryItem.Id;
                        item.InventoryItem = null;
                    }
                    
                    // Set the shipment ID and add the item
                    item.ShipmentId = shipment.Id;
                    _context.ShipmentInventoryItems.Add(item);
                }
                
                _context.SaveChanges();
            }
        }

        public void UpdateShipment(Shipment shipment)
        {
            // Avoid tracking conflicts by detaching any existing entity with the same ID
            var existingShipment = _context.Shipments.Local.FirstOrDefault(s => s.Id == shipment.Id);
            if (existingShipment != null)
            {
                _context.Entry(existingShipment).State = EntityState.Detached;
            }
            
            // Handle shipment items separately to avoid tracking conflicts
            var shipmentItems = shipment.ShipmentItems?.ToList();
            shipment.ShipmentItems = null;
            
            // Update the shipment
            _context.Shipments.Update(shipment);
            _context.SaveChanges();
            
            // Update shipment items if they exist
            if (shipmentItems != null && shipmentItems.Any())
            {
                // Remove existing items
                var existingItems = _context.ShipmentInventoryItems
                    .Where(si => si.ShipmentId == shipment.Id)
                    .ToList();
                
                _context.ShipmentInventoryItems.RemoveRange(existingItems);
                _context.SaveChanges();
                
                // Add the new items
                foreach (var item in shipmentItems)
                {
                    item.ShipmentId = shipment.Id;
                    _context.ShipmentInventoryItems.Add(item);
                }
                
                _context.SaveChanges();
            }
        }

        public void DeleteShipment(int id)
        {
            var shipment = _context.Shipments.Find(id);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
                _context.SaveChanges();
            }
        }

        public bool IsWithinTruckCapacity(Shipment shipment)
        {
            double totalWeight = CalculateTotalWeight(shipment);
            return totalWeight <= shipment.Truck.MaxCapacityKg;
        }

        public double CalculateTotalWeight(Shipment shipment)
        {
            // If ShipmentItems is null or empty, load it from the database
            if (shipment.ShipmentItems == null || !shipment.ShipmentItems.Any())
            {
                var loadedShipment = _context.Shipments
                    .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                    .FirstOrDefault(s => s.Id == shipment.Id);

                if (loadedShipment != null)
                {
                    shipment.ShipmentItems = loadedShipment.ShipmentItems;
                }
                else
                {
                    return 0; // No items or new shipment
                }
            }

            // Calculate total weight
            double totalWeight = 0;
            foreach (var item in shipment.ShipmentItems)
            {
                // Ensure InventoryItem is loaded
                if (item.InventoryItem == null)
                {
                    var inventoryItem = _context.InventoryItems.Find(item.InventoryItemId);
                    if (inventoryItem != null)
                    {
                        item.InventoryItem = inventoryItem;
                    }
                }

                if (item.InventoryItem != null)
                {
                    totalWeight += item.InventoryItem.WeightPerUnit * item.Quantity;
                }
            }

            return totalWeight;
        }

        public double CalculateLoadPercentage(Shipment shipment)
        {
            if (shipment.Truck == null)
            {
                var truck = _context.Trucks.Find(shipment.TruckId);
                if (truck == null) return 0;
                shipment.Truck = truck;
            }

            double totalWeight = CalculateTotalWeight(shipment);
            if (shipment.Truck.MaxCapacityKg <= 0) return 0;
            
            return Math.Round((totalWeight / shipment.Truck.MaxCapacityKg) * 100, 1);
        }
    }
}