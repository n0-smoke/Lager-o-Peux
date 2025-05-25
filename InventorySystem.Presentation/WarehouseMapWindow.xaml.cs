using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class WarehouseMapWindow : Window
    {
        private readonly AppDbContext _context;

        public WarehouseMapWindow()
        {
            InitializeComponent();

            // Set up the database context
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True")
                .Options;

            _context = new AppDbContext(options);

            LoadMapPage();
        }

        private async void LoadMapPage()
        {
            string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "MapPage.html");

            if (!File.Exists(mapPath))
            {
                MessageBox.Show("MapPage.html not found in Assets folder.");
                return;
            }

            await MapView.EnsureCoreWebView2Async();

            MapView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            MapView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

            MapView.CoreWebView2.Navigate(mapPath);
        }

        private void WebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = e.TryGetWebMessageAsString();

            if (message == "request-warehouses")
            {
                var warehouses = _context.Warehouses
                    .Select(w => new { id = w.Id, lat = w.Latitude, lng = w.Longitude, name = w.Name })
                    .ToList();

                string json = JsonSerializer.Serialize(warehouses);
                string js = $"window.setWarehouses({json});";
                _ = MapView.ExecuteScriptAsync(js);
            }
            else if (message.StartsWith("open-inventory-"))
            {
                var idStr = message.Replace("open-inventory-", "");
                if (int.TryParse(idStr, out int warehouseId))
                {
                    new WarehouseItemTable(warehouseId).Show(); // 👈 open the view
                }
            }
        }

    }
}
