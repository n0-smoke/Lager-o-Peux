using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;

namespace InventorySystem.Infrastructure.Services
{
    public class WarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public List<Warehouse> GetAllWarehouses()
        {
            return _context.Warehouses.OrderBy(w => w.Name).ToList();
        }

        public void AddWarehouse(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
        }

        public void DeleteWarehouse(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                _context.SaveChanges();
            }
        }
    }
}
