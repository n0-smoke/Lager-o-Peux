using System;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditShipmentWindow : Window
    {
        private readonly ShipmentService _shipmentService;
        private readonly Shipment _shipment;

        public EditShipmentWindow(Shipment shipment)
        {
            InitializeComponent();
            _shipment = shipment;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(context);

            // Pre-fill UI
            TruckIdBox.Text = _shipment.TruckId;
            DestinationBox.Text = _shipment.Destination;
            CapacityBox.Text = _shipment.LoadCapacity.ToString();

            // Set selected status
            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Content.ToString() == _shipment.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _shipment.TruckId = TruckIdBox.Text;
            _shipment.Destination = DestinationBox.Text;
            _shipment.LoadCapacity = int.TryParse(CapacityBox.Text, out int cap) ? cap : 0;
            _shipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";

            try
            {
                // Apply inventory logic if just marked as Delivered and not already applied
                if (_shipment.Status == "Delivered" && !_shipment.IsInventoryApplied)
                {
                    _shipmentService.ApplyShipmentToInventory(_shipment);
                }

                _shipmentService.UpdateShipment(_shipment);

                MessageBox.Show("Shipment updated successfully.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
