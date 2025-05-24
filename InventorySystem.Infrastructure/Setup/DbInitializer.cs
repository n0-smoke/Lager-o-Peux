using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Setup
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();

            // Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Username = "sarajevo", PasswordHash = BCrypt.Net.BCrypt.HashPassword("sarajevo123"), Location = "Sarajevo" },
                    new User { Username = "zagreb", PasswordHash = BCrypt.Net.BCrypt.HashPassword("zagreb123"), Location = "Zagreb" },
                    new User { Username = "belgrade", PasswordHash = BCrypt.Net.BCrypt.HashPassword("belgrade123"), Location = "Belgrade" }
                );
            }

            // Warehouses
            if (!context.Warehouses.Any())
            {
                context.Warehouses.AddRange(
                    new Warehouse { Name = "Sarajevo Central", Location = "Sarajevo", Capacity = 10000 },
                    new Warehouse { Name = "Zagreb Hub", Location = "Zagreb", Capacity = 9000 },
                    new Warehouse { Name = "Belgrade Depot", Location = "Belgrade", Capacity = 12000 }
                );
            }

            // Clients
            if (!context.Clients.Any())
            {
                context.Clients.AddRange(
                    new Client { Name = "Client A", Location = "Sarajevo" },
                    new Client { Name = "Client B", Location = "Zagreb" },
                    new Client { Name = "Client C", Location = "Belgrade" }
                );
            }

            // All 6 bidirectional routes
            if (!context.Routes.Any())
            {
                context.Routes.AddRange(
                    new Route { Location1 = "Sarajevo", Location2 = "Zagreb" },
                    new Route { Location1 = "Zagreb", Location2 = "Sarajevo" },

                    new Route { Location1 = "Zagreb", Location2 = "Belgrade" },
                    new Route { Location1 = "Belgrade", Location2 = "Zagreb" },

                    new Route { Location1 = "Sarajevo", Location2 = "Belgrade" },
                    new Route { Location1 = "Belgrade", Location2 = "Sarajevo" }
                );
                context.SaveChanges();
            }

            // Trucks per direction
            if (!context.Trucks.Any())
            {
                var routes = context.Routes.ToList();

                var rSZ = routes.First(r => r.Location1 == "Sarajevo" && r.Location2 == "Zagreb");
                var rZS = routes.First(r => r.Location1 == "Zagreb" && r.Location2 == "Sarajevo");

                var rZB = routes.First(r => r.Location1 == "Zagreb" && r.Location2 == "Belgrade");
                var rBZ = routes.First(r => r.Location1 == "Belgrade" && r.Location2 == "Zagreb");

                var rSB = routes.First(r => r.Location1 == "Sarajevo" && r.Location2 == "Belgrade");
                var rBS = routes.First(r => r.Location1 == "Belgrade" && r.Location2 == "Sarajevo");

                context.Trucks.AddRange(
                    // Sarajevo → Zagreb
                    new Truck { Name = "Truck S-Z 1", RouteId = rSZ.Id, LoadCapacity = 5000, Location = "Sarajevo", Availability = true, FuelConsumption = 12.5, Mileage = 34000 },
                    new Truck { Name = "Truck S-Z 2", RouteId = rSZ.Id, LoadCapacity = 6000, Location = "Sarajevo", Availability = true, FuelConsumption = 11.5, Mileage = 32000 },

                    // Zagreb → Sarajevo
                    new Truck { Name = "Truck Z-S 1", RouteId = rZS.Id, LoadCapacity = 4000, Location = "Zagreb", Availability = true, FuelConsumption = 10.0, Mileage = 42000 },

                    // Zagreb → Belgrade
                    new Truck { Name = "Truck Z-B 1", RouteId = rZB.Id, LoadCapacity = 6000, Location = "Zagreb", Availability = true, FuelConsumption = 10.5, Mileage = 39000 },

                    // Belgrade → Zagreb
                    new Truck { Name = "Truck B-Z 1", RouteId = rBZ.Id, LoadCapacity = 5500, Location = "Belgrade", Availability = true, FuelConsumption = 12.0, Mileage = 30000 },

                    // Sarajevo → Belgrade
                    new Truck { Name = "Truck S-B 1", RouteId = rSB.Id, LoadCapacity = 4500, Location = "Sarajevo", Availability = true, FuelConsumption = 13.0, Mileage = 31000 },

                    // Belgrade → Sarajevo
                    new Truck { Name = "Truck B-S 1", RouteId = rBS.Id, LoadCapacity = 4800, Location = "Belgrade", Availability = true, FuelConsumption = 11.8, Mileage = 32000 }
                );
            }

            // Inventory Items
            if (!context.InventoryItems.Any())
            {
                context.InventoryItems.AddRange(
                    new InventoryItem { Name = "Cement", Category = "Construction", WeightPerUnit = 25 },
                    new InventoryItem { Name = "Steel Beams", Category = "Construction", WeightPerUnit = 200 },
                    new InventoryItem { Name = "Wheat", Category = "Food", WeightPerUnit = 50 },
                    new InventoryItem { Name = "Electronics Kit", Category = "Electronics", WeightPerUnit = 5 },
                    new InventoryItem { Name = "Books", Category = "Education", WeightPerUnit = 2 }
                );
            }

            context.SaveChanges();

            // Fill inventory at each warehouse
            var warehouses = context.Warehouses.ToList();
            var items = context.InventoryItems.ToList();

            if (!context.WarehouseItems.Any())
            {
                context.WarehouseItems.AddRange(
                    new WarehouseItem { WarehouseId = warehouses[0].Id, InventoryItemId = items[0].Id, Amount = 100 },
                    new WarehouseItem { WarehouseId = warehouses[0].Id, InventoryItemId = items[1].Id, Amount = 40 },
                    new WarehouseItem { WarehouseId = warehouses[1].Id, InventoryItemId = items[2].Id, Amount = 200 },
                    new WarehouseItem { WarehouseId = warehouses[1].Id, InventoryItemId = items[3].Id, Amount = 150 },
                    new WarehouseItem { WarehouseId = warehouses[2].Id, InventoryItemId = items[4].Id, Amount = 300 }
                );
            }

            context.SaveChanges();
        }
    }
}
