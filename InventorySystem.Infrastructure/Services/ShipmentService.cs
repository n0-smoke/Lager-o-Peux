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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get a clean copy of the shipment without items
                    var shipmentToUpdate = new Shipment
                    {
                        Id = shipment.Id,
                        TruckId = shipment.TruckId,
                        Destination = shipment.Destination,
                        Direction = shipment.Direction,
                        Status = shipment.Status,
                        ScheduledDate = shipment.ScheduledDate
                    };
                    
                    // Handle shipment items separately
                    var shipmentItems = shipment.ShipmentItems?.ToList() ?? new List<ShipmentInventoryItem>();
                    
                    // Update the shipment
                    _context.Entry(shipmentToUpdate).State = EntityState.Modified;
                    _context.SaveChanges();
                    
                    // Remove all existing items for this shipment
                    _context.ShipmentInventoryItems.RemoveRange(
                        _context.ShipmentInventoryItems.Where(si => si.ShipmentId == shipment.Id));
                    _context.SaveChanges();
                    
                    // Add the new items
                    foreach (var item in shipmentItems)
                    {
                        // Create a clean item to avoid tracking issues
                        var newItem = new ShipmentInventoryItem
                        {
                            ShipmentId = shipment.Id,
                            InventoryItemId = item.InventoryItemId,
                            Quantity = item.Quantity
                        };
                        
                        _context.ShipmentInventoryItems.Add(newItem);
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Error updating shipment: {ex.Message}", ex);
                }
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
            // If ShipmentItems is null, initialize it as an empty list
            if (shipment.ShipmentItems == null)
            {
                shipment.ShipmentItems = new List<ShipmentInventoryItem>();
            }
            
            // If the list is explicitly empty (items were removed), return 0 immediately
            if (!shipment.ShipmentItems.Any())
            {
                return 0;
            }
            
            // For existing shipments with no items loaded yet, try to load from database
            if (shipment.Id > 0 && shipment.ShipmentItems.Count == 0)
            {
                var loadedShipment = _context.Shipments
                    .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                    .FirstOrDefault(s => s.Id == shipment.Id);

                if (loadedShipment != null && loadedShipment.ShipmentItems.Any())
                {
                    shipment.ShipmentItems = loadedShipment.ShipmentItems;
                }
                else
                {
                    return 0; // No items found in database
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