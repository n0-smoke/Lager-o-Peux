using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InventorySystem.Presentation
{
    public partial class ShipmentOverviewWindow : Window
    {
        private readonly AppDbContext _context;

        public ShipmentOverviewWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");
            _context = new AppDbContext(optionsBuilder.Options);

            LoadShipments();
        }

        private void LoadShipments()
        {
            var shipments = _context.Shipments
                .Include(s => s.Truck)
                .Include(s => s.Route)
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                .ToList()
                .Select(s => new ShipmentOverviewViewModel
                {
                    Id = s.Id,
                    Truck = s.Truck,
                    RouteDisplay = $"{s.Route.Location1} ➜ {s.Route.Location2}",
                    TotalWeight = s.ShipmentItems.Sum(si => si.Amount * si.InventoryItem.WeightPerUnit)
                }).ToList();

            ShipmentsGrid.ItemsSource = shipments;
        }

        private void AddShipment_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddShipmentWindow(); // Should support Add or Edit mode
            addWindow.ShowDialog();
            LoadShipments(); // Refresh after close
        }

        private void EditShipment_Click(object sender, RoutedEventArgs e)
        {
            if (ShipmentsGrid.SelectedItem is not ShipmentOverviewViewModel selected) return;

            var editWindow = new AddShipmentWindow(selected.Id); // ✅ Constructor that loads existing shipment
            editWindow.ShowDialog();
            LoadShipments(); // Refresh
        }

        private async void DeleteShipment_Click(object sender, RoutedEventArgs e)
        {
            if (ShipmentsGrid.SelectedItem is not ShipmentOverviewViewModel selected) return;

            var result = MessageBox.Show("Are you sure you want to delete this shipment?", "Confirm", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                var trackedShipment = await _context.Shipments
                    .Include(s => s.ShipmentItems)
                    .FirstOrDefaultAsync(s => s.Id == selected.Id);

                if (trackedShipment != null)
                {
                    _context.ShipmentItems.RemoveRange(trackedShipment.ShipmentItems);
                    _context.Shipments.Remove(trackedShipment);
                    await _context.SaveChangesAsync();
                    LoadShipments();
                }
                else
                {
                    MessageBox.Show("Shipment not found or already deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                MessageBox.Show("This shipment was already modified or deleted by another operation.", "Concurrency Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the shipment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class ShipmentOverviewViewModel
    {
        public int Id { get; set; }
        public Truck Truck { get; set; }
        public string RouteDisplay { get; set; }
        public double TotalWeight { get; set; }
    }
}
