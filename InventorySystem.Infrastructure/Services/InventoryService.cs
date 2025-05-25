using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        // Get all inventory items (optionally include warehouse/shipment data if needed)
        public List<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems
                .Include(i => i.WarehouseItems)
                .Include(i => i.ShipmentItems)
                .OrderBy(i => i.Name)
                .ToList();
        }

        // Get a specific item by ID
        public InventoryItem? GetItemById(int id)
        {
            return _context.InventoryItems
                .Include(i => i.WarehouseItems)
                .Include(i => i.ShipmentItems)
                .FirstOrDefault(i => i.Id == id);
        }

        // Add a new inventory item
        public void AddItem(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
        }

        // Update an existing inventory item
        public void UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.SaveChanges();
        }

        // Delete an inventory item by ID
        public void DeleteItem(int id)
        {
            var item = _context.InventoryItems
                .Include(i => i.WarehouseItems)
                .Include(i => i.ShipmentItems)
                .FirstOrDefault(i => i.Id == id);

            if (item != null)
            {
                // If cascade delete is not configured, manually remove dependencies first
                _context.InventoryItems.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
