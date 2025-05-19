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

        private Shipment _newShipment;
        
        public AddShipmentWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);
            
            // Initialize a new shipment with empty items collection
            _newShipment = new Shipment
            {
                ShipmentItems = new List<ShipmentInventoryItem>(),
                CreatedAt = DateTime.UtcNow
            };
            
            LoadTrucks();
            
            // Set up event handlers
            TruckComboBox.SelectionChanged += (s, e) => UpdateCapacityDisplay();
        }

        private void LoadTrucks()
        {
            var trucks = _context.Trucks.ToList();
            TruckComboBox.ItemsSource = trucks;
        }
        
        private void ManageItems_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck before managing shipment items.");
                return;
            }

            // Update the shipment's truck reference
            _newShipment.TruckId = selectedTruck.Id;
            _newShipment.Truck = selectedTruck;
            _newShipment.Destination = DestinationBox.Text;
            _newShipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";

            // Open the manage shipment items window
            var manageItemsWindow = new ManageShipmentItemsWindow(_newShipment, true);
            bool? result = manageItemsWindow.ShowDialog();

            if (result == true)
            {
                // Update the DataGrid
                ShipmentItemsGrid.ItemsSource = null;
                ShipmentItemsGrid.ItemsSource = _newShipment.ShipmentItems;
                
                // Update capacity display
                UpdateCapacityDisplay();
            }
        }

        private void UpdateCapacityDisplay()
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                // Reset display if no truck is selected
                LoadPercentageText.Text = "0%";
                LoadProgressBar.Value = 0;
                CapacityWarningText.Visibility = Visibility.Collapsed;
                return;
            }
            
            // Update the shipment's truck reference for calculation
            _newShipment.TruckId = selectedTruck.Id;
            _newShipment.Truck = selectedTruck;
            
            // Calculate load percentage
            double loadPercentage = _shipmentService.CalculateLoadPercentage(_newShipment);
            
            // Update UI
            LoadPercentageText.Text = $"{loadPercentage}%";
            LoadProgressBar.Value = loadPercentage;
            
            // Set color based on load percentage
            if (loadPercentage > 100)
            {
                LoadProgressBar.Foreground = new SolidColorBrush(Colors.Red);
                CapacityWarningText.Visibility = Visibility.Visible;
            }
            else if (loadPercentage > 90)
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

            _newShipment.TruckId = selectedTruck.Id;
            _newShipment.Truck = selectedTruck;
            _newShipment.Destination = DestinationBox.Text;
            _newShipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";
            
            // Validate truck capacity
            if (_newShipment.ShipmentItems.Any() && !_shipmentService.IsWithinTruckCapacity(_newShipment))
            {
                var result = MessageBox.Show(
                    "Warning: This shipment exceeds the truck's capacity. Do you want to continue anyway?",
                    "Capacity Exceeded",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.No)
                {
                    return; // Don't save if user cancels
                }
            }

            _shipmentService.AddShipment(_newShipment);

            MessageBox.Show("Shipment added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}

