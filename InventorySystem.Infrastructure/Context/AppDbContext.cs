using InventorySystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

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

    //extras for Truck
    public DbSet<Truck> Trucks { get; set; }
    public IEnumerable<object> TruckMaintenanceRecords { get; internal set; }

    public DbSet<TruckMaintenanceRecord> TruckM;
}
