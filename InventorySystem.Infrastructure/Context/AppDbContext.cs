using InventorySystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configs
            modelBuilder.Entity<WarehouseItem>()
                .HasOne(wi => wi.InventoryItem)
                .WithMany(ii => ii.WarehouseItems)
                .HasForeignKey(wi => wi.InventoryItemId);

            modelBuilder.Entity<WarehouseItem>()
                .HasOne(wi => wi.Warehouse)
                .WithMany(w => w.WarehouseItems)
                .HasForeignKey(wi => wi.WarehouseId);

            modelBuilder.Entity<ShipmentItem>()
                .HasOne(si => si.InventoryItem)
                .WithMany(ii => ii.ShipmentItems)
                .HasForeignKey(si => si.InventoryItemId);

            modelBuilder.Entity<ShipmentItem>()
                .HasOne(si => si.Shipment)
                .WithMany(s => s.ShipmentItems)
                .HasForeignKey(si => si.ShipmentId);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Truck)
                .WithMany(t => t.Shipments)
                .HasForeignKey(s => s.TruckId)
                .OnDelete(DeleteBehavior.Restrict); // ❌ No cascade

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Shipments)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict); // Optional: also restrict here

            modelBuilder.Entity<Truck>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trucks)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Important!


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Enum conversion for RouteDirection
            modelBuilder.Entity<Route>()
                .Property(r => r.Direction)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
