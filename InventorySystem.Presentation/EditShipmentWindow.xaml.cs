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
    public partial class EditShipmentWindow : Window
    {
        private readonly ShipmentService _shipmentService;
        private readonly AppDbContext _context;
        private readonly Shipment _shipment;

        public EditShipmentWindow(Shipment shipment)
        {
            InitializeComponent();
            _shipment = shipment;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);

            LoadTrucks();

            DestinationBox.Text = _shipment.Destination;
            TruckComboBox.SelectedValue = _shipment.TruckId;

            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Content?.ToString() == _shipment.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void LoadTrucks()
        {
            var trucks = _context.Trucks.ToList();
            TruckComboBox.ItemsSource = trucks;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            _shipment.TruckId = selectedTruck.Id;
            _shipment.Destination = DestinationBox.Text;
            _shipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";

            _shipmentService.UpdateShipment(_shipment);

            MessageBox.Show("Shipment updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
