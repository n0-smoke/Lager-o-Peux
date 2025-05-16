using System;
using System.Linq;
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
        private readonly AppDbContext _context;
        private readonly ShipmentService _shipmentService;

        public AddShipmentWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);

            LoadTrucks();
        }

        private void LoadTrucks()
        {
            var trucks = _context.Trucks.ToList();
            TruckComboBox.ItemsSource = trucks;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            var shipment = new Shipment
            {
                TruckId = selectedTruck.Id,
                Truck = selectedTruck,
                Destination = DestinationBox.Text,
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

