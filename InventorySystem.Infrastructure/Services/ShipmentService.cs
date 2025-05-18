using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
        }

        public void UpdateShipment(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            _context.SaveChanges();
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

        public void ApplyShipmentToInventory(Shipment shipment)
        {
            
            if (shipment.IsInventoryApplied)
                return;

            // Load shipment with related items and inventory items
            var shipmentWithItems = _context.Shipments
                .Where(s => s.Id == shipment.Id)
                .Include(s => s.Items)
                .ThenInclude(si => si.InventoryItem)
                .FirstOrDefault();

            if (shipmentWithItems == null || shipmentWithItems.Items == null)
                return;

            foreach (var shipmentItem in shipmentWithItems.Items)
            {
                var inventoryItem = shipmentItem.InventoryItem;

                if (inventoryItem == null)
                    continue;

                //If in incoming, increase quantity, if outgoing, decrease quantity
                // there is a check for outgoing, in case the item is out of stock

                if (shipment.Direction == "Incoming")
                {
                    inventoryItem.Quantity += shipmentItem.Quantity;
                }
                else if (shipment.Direction == "Outgoing")
                {
                    if (inventoryItem.Quantity < shipmentItem.Quantity)
                    {
                        throw new InvalidOperationException($"Not enough stock of '{inventoryItem.Name}' to fulfill shipment.");
                    }

                    inventoryItem.Quantity -= shipmentItem.Quantity;
                }
            }

            // Mark the shipment as applied to avoid repeat adjustment and save changes
            shipmentWithItems.IsInventoryApplied = true;

            _context.SaveChanges();
        }

    }
}