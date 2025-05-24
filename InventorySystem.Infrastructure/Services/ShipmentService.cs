using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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
            return _context.Shipments
                .Include(s => s.Truck)
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
        }

        public void AddShipment(Shipment shipment)
        {
            var shipmentItems = shipment.ShipmentItems?.ToList();
            shipment.ShipmentItems = null;

            _context.Shipments.Add(shipment);
            _context.SaveChanges();

            if (shipmentItems != null && shipmentItems.Any())
            {
                foreach (var item in shipmentItems)
                {
                    if (item.InventoryItem != null)
                    {
                        item.InventoryItemId = item.InventoryItem.Id;
                        item.InventoryItem = null;
                    }

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
                    var shipmentToUpdate = new Shipment
                    {
                        Id = shipment.Id,
                        TruckId = shipment.TruckId,
                        Destination = shipment.Destination,
                        Direction = shipment.Direction,
                        Status = shipment.Status,
                        ScheduledDate = shipment.ScheduledDate,
                        IsInventoryApplied = shipment.IsInventoryApplied
                        // CreatedAt is not updated manually
                    };


                    var shipmentItems = shipment.ShipmentItems?.ToList() ?? new List<ShipmentInventoryItem>();

                    _context.Entry(shipmentToUpdate).State = EntityState.Modified;
                    _context.SaveChanges();

                    _context.ShipmentInventoryItems.RemoveRange(
                        _context.ShipmentInventoryItems.Where(si => si.ShipmentId == shipment.Id));
                    _context.SaveChanges();

                    foreach (var item in shipmentItems)
                    {
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
            if (shipment.ShipmentItems == null)
                shipment.ShipmentItems = new List<ShipmentInventoryItem>();

            if (!shipment.ShipmentItems.Any())
                return 0;

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
                    return 0;
                }
            }

            double totalWeight = 0;
            foreach (var item in shipment.ShipmentItems)
            {
                if (item.InventoryItem == null)
                {
                    item.InventoryItem = _context.InventoryItems.Find(item.InventoryItemId);
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
                shipment.Truck = _context.Trucks.Find(shipment.TruckId);
                if (shipment.Truck == null || shipment.Truck.MaxCapacityKg <= 0)
                    return 0;
            }

            double totalWeight = CalculateTotalWeight(shipment);
            return Math.Round((totalWeight / shipment.Truck.MaxCapacityKg) * 100, 1);
        }

        public void ApplyShipmentToInventory(Shipment shipment)
        {
            if (shipment.IsInventoryApplied)
                return;

            _context.Entry(shipment).Collection(s => s.ShipmentItems).Load();

            foreach (var item in shipment.ShipmentItems)
            {
                _context.Entry(item).Reference(i => i.InventoryItem).Load();

                if (shipment.Direction == ShipmentDirection.Incoming)
                {
                    item.InventoryItem.Quantity += item.Quantity;
                }
                else if (shipment.Direction == ShipmentDirection.Outgoing)
                {
                    if (item.InventoryItem.Quantity < item.Quantity)
                        throw new InvalidOperationException("Not enough inventory for shipment.");

                    item.InventoryItem.Quantity -= item.Quantity;
                }
            }

            shipment.IsInventoryApplied = true;
            _context.SaveChanges();
        }
    }
}
