using System;
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

        // Add a new truck
        public void AddTruck(Truck truck)
        {
            _context.Trucks.Add(truck);
            _context.SaveChanges();
        }

        // Get all trucks
        public List<Truck> GetAllTrucks()
        {
            return _context.Trucks.ToList();
        }

        // Find a truck by ID
        public Truck? FindTruckById(int truckId)
        {
            return _context.Trucks.Find(truckId);
        }

        // Get maintenance records for a truck
        public List<TruckMaintenanceRecord> GetMaintenanceForTruck(int truckId)
        {
            return _context.TruckMaintenanceRecords
                .Where(r => r.TruckId == truckId)
                .OrderByDescending(r => r.StartDate)
                .ToList();
        }

        // Add a maintenance record
        public void AddMaintenanceRecord(TruckMaintenanceRecord record)
        {
            _context.TruckMaintenanceRecords.Add(record);

            var truck = _context.Trucks.FirstOrDefault(t => t.Id == record.TruckId);
            if (truck != null)
            {
                truck.IsUnderMaintenance = true;
                truck.Status = "Unavailable";
                truck.LastMaintenanceDate = record.StartDate;
            }

            _context.SaveChanges();
        }

        // Complete a maintenance task
        public void CompleteMaintenance(int recordId)
        {
            var record = _context.TruckMaintenanceRecords
                .Include(r => r.Truck)
                .FirstOrDefault(r => r.Id == recordId);

            if (record != null && record.Status != MaintenanceStatus.Completed)
            {
                record.Status = MaintenanceStatus.Completed;
                record.EndDate = DateTime.Now;

                _context.TruckMaintenanceRecords.Update(record); // Ensure record is updated

                if (record.Truck != null)
                {
                    var truck = _context.Trucks.First(t => t.Id == record.TruckId); // Ensure fresh tracked instance

                    bool stillActive = _context.TruckMaintenanceRecords
                        .Any(r => r.TruckId == truck.Id && r.Status != MaintenanceStatus.Completed);

                    truck.IsUnderMaintenance = stillActive;
                    truck.Status = stillActive ? "Unavailable" : "Available";

                    _context.Trucks.Update(truck); // Ensure EF Core tracks the truck update
                }

                _context.SaveChanges(); // Persist both truck and record changes
            }
        }


        // Get all maintenance records (admin use)
        public List<TruckMaintenanceRecord> GetAllMaintenanceRecords()
        {
            return _context.TruckMaintenanceRecords
                .Include(r => r.Truck)
                .OrderByDescending(r => r.StartDate)
                .ToList();
        }

        // Find an available truck (not under maintenance)
        public Truck? FindAvailableTruck()
        {
            return _context.Trucks
                .FirstOrDefault(t => !t.IsUnderMaintenance && t.Status == "Available");
        }

        // Update status manually
        public void UpdateTruckStatus(int truckId, string newStatus)
        {
            var truck = _context.Trucks.Find(truckId);
            if (truck != null)
            {
                truck.Status = newStatus;
                _context.SaveChanges();
            }
        }
    }
}
