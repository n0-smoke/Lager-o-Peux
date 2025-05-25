using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Services
{
    public class TruckService
    {
        private readonly AppDbContext _context;

        public TruckService(AppDbContext context)
        {
            _context = context;
        }

        public List<Truck> GetAllTrucks()
        {
            return _context.Trucks
                .Include(t => t.Route)
                .Include(t => t.Shipments)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public Truck? GetTruckById(int id)
        {
            return _context.Trucks
                .Include(t => t.Route)
                .Include(t => t.Shipments)
                .FirstOrDefault(t => t.Id == id);
        }

        public void AddTruck(Truck truck)
        {
            _context.Trucks.Add(truck);
            _context.SaveChanges();
        }

        public void UpdateTruck(Truck truck)
        {
            _context.Trucks.Update(truck);
            _context.SaveChanges();
        }

        public void DeleteTruck(int id)
        {
            var truck = _context.Trucks.Find(id);
            if (truck != null)
            {
                _context.Trucks.Remove(truck);
                _context.SaveChanges();
            }
        }
    }
}