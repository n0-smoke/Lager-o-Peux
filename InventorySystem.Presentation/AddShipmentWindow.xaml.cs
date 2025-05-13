using System;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddShipmentWindow : Window
    {
        private readonly ShipmentService _shipmentService;

        public AddShipmentWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(context);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var shipment = new Shipment
            {
                TruckId = TruckIdBox.Text,
                Destination = DestinationBox.Text,
                LoadCapacity = int.TryParse(CapacityBox.Text, out int cap) ? cap : 0,
                Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _shipmentService.AddShipment(shipment);

            MessageBox.Show("Shipment added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
