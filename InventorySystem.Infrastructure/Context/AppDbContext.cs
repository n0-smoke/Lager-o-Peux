using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Domain.Models; // adjust to your actual model namespace

namespace InventorySystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TruckMaintenanceRecord> TruckMaintenanceRecords { get; set; }
        public DbSet<ShipmentInventoryItem> ShipmentInventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShipmentInventoryItem>()
                .HasKey(x => new { x.ShipmentId, x.InventoryItemId });

            modelBuilder.Entity<ShipmentInventoryItem>()
                .HasOne(x => x.Shipment)
                .WithMany(s => s.ShipmentItems)
                .HasForeignKey(x => x.ShipmentId);

            modelBuilder.Entity<ShipmentInventoryItem>()
                .HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId);
            modelBuilder.Entity<Truck>()
                .HasMany(t => t.MaintenanceRecords)
                .WithOne(m => m.Truck)
                .HasForeignKey(m => m.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}

