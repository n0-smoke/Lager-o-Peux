using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventorySystem.Application.Services;
using InventorySystem.Infrastructure.Services;

public static class DbConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register shipment service
        services.AddScoped<IShipmentService, ShipmentService>();

        return services;
    }
}
