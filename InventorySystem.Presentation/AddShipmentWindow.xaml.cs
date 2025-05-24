using InventorySystem.Domain.Models;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Presentation.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InventorySystem.Presentation
{
    public partial class AddShipmentWindow : Window
    {
        private readonly AppDbContext _context;
        private List<InventoryItem> inventoryItems;
        private List<Truck> trucks;
        private List<ShipmentItemViewModel> shipmentItems = new();

        public AddShipmentWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");
            _context = new AppDbContext(optionsBuilder.Options);

            DestinationTypeComboBox.SelectedIndex = 0;
            LoadInventoryItems();
        }

        private void LoadInventoryItems()
        {
            inventoryItems = _context.InventoryItems.ToList();
            InventoryItemComboBox.ItemsSource = inventoryItems;
            InventoryItemComboBox.DisplayMemberPath = "Name";
        }

        private void DestinationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DestinationTypeComboBox.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Content.ToString() == "Warehouse")
                {
                    DestinationEntityComboBox.ItemsSource = _context.Warehouses.ToList();
                    DestinationEntityComboBox.DisplayMemberPath = "Name";
                }
                else
                {
                    DestinationEntityComboBox.ItemsSource = _context.Clients.ToList();
                    DestinationEntityComboBox.DisplayMemberPath = "Name";
                }
            }
        }

        private void DestinationEntityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DestinationEntityComboBox.SelectedItem == null) return;

            string destinationLocation = DestinationTypeComboBox.Text == "Warehouse"
                ? ((Warehouse)DestinationEntityComboBox.SelectedItem).Location
                : ((Client)DestinationEntityComboBox.SelectedItem).Location;

            string currentLocation = SessionManager.CurrentUser?.Location ?? "Sarajevo";

            var route = _context.Routes.FirstOrDefault(r => r.Location1 == currentLocation && r.Location2 == destinationLocation);
            if (route == null)
            {
                MessageBox.Show("No route found between current location and destination.");
                return;
            }

            trucks = _context.Trucks
                .Where(t => t.RouteId == route.Id && t.Location == currentLocation && t.Availability)
                .ToList();

            // Include capacity in label
            TruckComboBox.ItemsSource = trucks.Select(t => new { Truck = t, Label = $"{t.Name} ({t.LoadCapacity} kg)" }).ToList();
            TruckComboBox.DisplayMemberPath = "Label";
            TruckComboBox.SelectedValuePath = "Truck";
            TruckComboBox.SelectedIndex = 0;
        }

        private void TruckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTruckLoadDisplay();
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryItemComboBox.SelectedItem is not InventoryItem item || !int.TryParse(QuantityTextBox.Text, out int qty) || qty <= 0)
                return;

            double weight = item.WeightPerUnit * qty;

            shipmentItems.Add(new ShipmentItemViewModel
            {
                ItemName = item.Name,
                Quantity = qty,
                Weight = weight
            });

            ShipmentItemsGrid.ItemsSource = null;
            ShipmentItemsGrid.ItemsSource = shipmentItems;

            UpdateTruckLoadDisplay();
        }

        private void UpdateTruckLoadDisplay()
        {
            if (TruckComboBox.SelectedItem == null) return;

            // Extract selected truck from anonymous type
            var selected = TruckComboBox.SelectedItem;
            Truck selectedTruck = selected.GetType().GetProperty("Truck")?.GetValue(selected) as Truck;

            if (selectedTruck == null) return;

            double currentWeight = shipmentItems.Sum(i => i.Weight);
            double capacity = selectedTruck.LoadCapacity;

            TruckLoadText.Text = $"{currentWeight} / {capacity} kg";
            TruckLoadProgressBar.Value = Math.Min(100, (currentWeight / capacity) * 100);
        }

        private void SubmitShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Implementation of actual shipment persistence logic...
            MessageBox.Show("Shipment submitted (not implemented)");
        }
    }

    public class ShipmentItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
    }
}
