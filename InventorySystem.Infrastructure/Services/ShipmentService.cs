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
    }
}