using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Setup
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Apply any pending migrations
            context.Database.Migrate();

            // Seed users if they don't exist
            SeedUsers(context);
            
            // Seed trucks if they don't exist
            SeedTrucks(context);
            
            // Seed inventory items if they don't exist
            SeedInventoryItems(context);
        }
        
        private static void SeedUsers(AppDbContext context)
        {
            // Check if users already exist
            if (context.Users.Any()) return;

            // Example password hash (you'll replace with real hash logic)
            string fakeHash = BCrypt.Net.BCrypt.HashPassword("admin123");

            context.Users.AddRange(
                new User { Username = "admin", PasswordHash = fakeHash, Role = "Admin" },
                new User { Username = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"), Role = "Manager" },
                new User { Username = "employee", PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee123"), Role = "Employee" }
            );

            context.SaveChanges();
        }
        
        private static void SeedTrucks(AppDbContext context)
        {
            // Check if trucks already exist
            if (context.Trucks.Any()) return;
            
            context.Trucks.AddRange(
                new Truck { Identifier = "TRK-001", MaxCapacityKg = 5000 },  // 5 tons
                new Truck { Identifier = "TRK-002", MaxCapacityKg = 10000 }, // 10 tons
                new Truck { Identifier = "TRK-003", MaxCapacityKg = 20000 }, // 20 tons
                new Truck { Identifier = "TRK-004", MaxCapacityKg = 2500 }   // 2.5 tons (smaller truck)
            );
            
            context.SaveChanges();
        }
        
        private static void SeedInventoryItems(AppDbContext context)
        {
            // Check if inventory items already exist
            if (context.InventoryItems.Any()) return;
            
            context.InventoryItems.AddRange(
                new InventoryItem { 
                    Name = "Steel Beams", 
                    Category = "Construction", 
                    Quantity = 50, 
                    Supplier = "Steel Corp", 
                    WeightPerUnit = 250.0, // 250kg per beam
                    ReorderThreshold = 10
                },
                new InventoryItem { 
                    Name = "Cement Bags", 
                    Category = "Construction", 
                    Quantity = 200, 
                    Supplier = "BuildWell", 
                    WeightPerUnit = 25.0, // 25kg per bag
                    ReorderThreshold = 20
                },
                new InventoryItem { 
                    Name = "Office Chairs", 
                    Category = "Furniture", 
                    Quantity = 30, 
                    Supplier = "Office Solutions", 
                    WeightPerUnit = 15.0, // 15kg per chair
                    ReorderThreshold = 5
                },
                new InventoryItem { 
                    Name = "Desktop Computers", 
                    Category = "Electronics", 
                    Quantity = 25, 
                    Supplier = "TechWorld", 
                    WeightPerUnit = 10.0, // 10kg per computer
                    ReorderThreshold = 5
                },
                new InventoryItem { 
                    Name = "Pallets of Bricks", 
                    Category = "Construction", 
                    Quantity = 40, 
                    Supplier = "Brick Masters", 
                    WeightPerUnit = 500.0, // 500kg per pallet
                    ReorderThreshold = 5
                }
            );
            
            context.SaveChanges();
        }
    }
}

