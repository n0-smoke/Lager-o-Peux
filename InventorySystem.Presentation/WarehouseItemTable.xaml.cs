using System.Windows;
using System.Linq;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class WarehouseItemTable : Window
    {
        public WarehouseItemTable(int warehouseId)
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True")
                .Options;

            using var context = new AppDbContext(options);

            var items = context.WarehouseItems
                .Where(wi => wi.WarehouseId == warehouseId)
                .Include(wi => wi.InventoryItem)
                .Select(wi => new
                {
                    ItemName = wi.InventoryItem.Name,
                    wi.Amount
                })
                .ToList();

            ItemGrid.ItemsSource = items;
        }
    }
}
