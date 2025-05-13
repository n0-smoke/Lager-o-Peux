using System.Windows;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Setup;
using WpfApplication = System.Windows.Application;

namespace InventorySystem.Presentation
{
    public partial class App : WpfApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up EF Core options
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            // Initialize the database and seed users if needed
            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                DbInitializer.Initialize(context);
            }

            // Launch only the login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}


