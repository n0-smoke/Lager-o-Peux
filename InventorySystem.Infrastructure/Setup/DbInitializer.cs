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
    }
}

