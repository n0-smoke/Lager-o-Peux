using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private List<ShipmentInventoryItem> assignedItems = new();

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

        private void TruckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLoadUI();
        }

        private void AssignItems_Click(object sender, RoutedEventArgs e)
        {
            var assignWindow = new AssignItemsWindow();
            bool? result = assignWindow.ShowDialog();

            if (result == true)
            {
                assignedItems = assignWindow.SelectedItems;
                MessageBox.Show($"{assignedItems.Count} item(s) assigned to this shipment.");
                UpdateLoadUI();
            }
        }

        private void UpdateLoadUI()
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck || assignedItems.Count == 0)
            {
                LoadPercentageText.Text = "0%";
                LoadProgressBar.Value = 0;
                CapacityWarningText.Visibility = Visibility.Collapsed;
                return;
            }

            var shipment = new Shipment
            {
                TruckId = selectedTruck.Id,
                Truck = selectedTruck,
                ShipmentItems = assignedItems
            };

            double load = _shipmentService.CalculateLoadPercentage(shipment);
            LoadPercentageText.Text = $"{load}%";
            LoadProgressBar.Value = load;

            if (load > 100)
            {
                LoadProgressBar.Foreground = new SolidColorBrush(Colors.Red);
                CapacityWarningText.Visibility = Visibility.Visible;
            }
            else if (load > 90)
            {
                LoadProgressBar.Foreground = new SolidColorBrush(Colors.Orange);
                CapacityWarningText.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadProgressBar.Foreground = new SolidColorBrush(Colors.Green);
                CapacityWarningText.Visibility = Visibility.Collapsed;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            if (selectedTruck.IsUnderMaintenance || selectedTruck.Status != "Available")
            {
                MessageBox.Show("This truck is currently unavailable and cannot be assigned to a shipment.");
                return;
            }

            var directionString = ((ComboBoxItem)DirectionBox.SelectedItem)?.Content?.ToString();
            var parsed = Enum.TryParse<ShipmentDirection>(directionString, out var shipmentDirection);
            if (!parsed)
            {
                MessageBox.Show("Please select a shipment direction.");
                return;
            }

            if (assignedItems.Count == 0)
            {
                var confirm = MessageBox.Show("No items are assigned. Continue anyway?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm != MessageBoxResult.Yes)
                    return;
            }

            var shipment = new Shipment
            {
                TruckId = selectedTruck.Id,
                Truck = selectedTruck,
                Destination = DestinationBox.Text,
                Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending",
                Direction = shipmentDirection,
                CreatedAt = DateTime.UtcNow,
                ShipmentItems = assignedItems
            };

            if (!_shipmentService.IsWithinTruckCapacity(shipment))
            {
                var confirmOverload = MessageBox.Show("Warning: Shipment exceeds truck capacity. Proceed anyway?", "Overload Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirmOverload != MessageBoxResult.Yes)
                    return;
            }

            _shipmentService.AddShipment(shipment);

            MessageBox.Show("Shipment added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
