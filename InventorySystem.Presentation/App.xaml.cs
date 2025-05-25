using System.Windows;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Setup;

namespace InventorySystem.Presentation
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up EF Core DbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                DbInitializer.Initialize(context);
            }

            // Launch main window
            // var loginWindow = new LoginWindow();
            // loginWindow.Show();
        }
    }
}
