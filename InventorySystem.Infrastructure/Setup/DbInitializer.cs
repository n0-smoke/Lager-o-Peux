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
            // Apply migrations to ensure DB is up to date
            context.Database.Migrate();

            // Skip if users already seeded
            if (context.Users.Any()) return;

            // Seed users representing different locations
            context.Users.AddRange(
                new User
                {
                    Username = "sarajevo",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("sarajevo123"),
                    Location = "Sarajevo"
                },
                new User
                {
                    Username = "zagreb",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("zagreb123"),
                    Location = "Zagreb"
                },
                new User
                {
                    Username = "belgrade",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("belgrade123"),
                    Location = "Belgrade"
                }
            );

            context.SaveChanges();
        }
    }
}
