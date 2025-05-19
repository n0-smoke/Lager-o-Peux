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
    public partial class ManageShipmentItemsWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly ShipmentService _shipmentService;
        private readonly Shipment _shipment;
        private readonly List<ShipmentInventoryItem> _shipmentItems;
        private readonly bool _isNewShipment;

        public ManageShipmentItemsWindow(Shipment shipment, bool isNewShipment = false)
        {
            InitializeComponent();
            _shipment = shipment;
            _isNewShipment = isNewShipment;

            // Initialize the shipment items collection
            _shipmentItems = new List<ShipmentInventoryItem>();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);

            // Load existing shipment items if this is an existing shipment
            if (!_isNewShipment)
            {
                LoadExistingShipmentItems();
            }

            LoadInventoryItems();
            UpdateCapacityDisplay();
        }

        private void LoadExistingShipmentItems()
        {
            // Load shipment with its items and related inventory items
            var shipmentWithItems = _context.Shipments
                .Include(s => s.ShipmentItems)
                .ThenInclude(si => si.InventoryItem)
                .FirstOrDefault(s => s.Id == _shipment.Id);

            if (shipmentWithItems != null && shipmentWithItems.ShipmentItems != null)
            {
                foreach (var item in shipmentWithItems.ShipmentItems)
                {
                    _shipmentItems.Add(item);
                }
            }

            ShipmentItemsGrid.ItemsSource = null;
            ShipmentItemsGrid.ItemsSource = _shipmentItems;
        }

        private void LoadInventoryItems()
        {
            var inventoryItems = _context.InventoryItems.ToList();
            InventoryItemsGrid.ItemsSource = inventoryItems;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = InventoryItemsGrid.SelectedItem as InventoryItem;
            
            if (selectedItem == null)
            {
                MessageBox.Show("Please select an inventory item to add.");
                return;
            }

            // Parse quantity
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity (greater than 0).");
                return;
            }

            // Check if there's enough inventory
            if (quantity > selectedItem.Quantity)
            {
                MessageBox.Show($"Not enough inventory. Only {selectedItem.Quantity} units available.");
                return;
            }

            // Check if item already exists in shipment
            var existingItem = _shipmentItems.FirstOrDefault(si => si.InventoryItemId == selectedItem.Id);
            
            if (existingItem != null)
            {
                // Update quantity of existing item
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add new item to shipment
                var shipmentItem = new ShipmentInventoryItem
                {
                    ShipmentId = _shipment.Id,
                    InventoryItemId = selectedItem.Id,
                    InventoryItem = selectedItem,
                    Quantity = quantity
                };
                
                _shipmentItems.Add(shipmentItem);
            }

            // Update the UI
            ShipmentItemsGrid.ItemsSource = null;
            ShipmentItemsGrid.ItemsSource = _shipmentItems;
            
            UpdateCapacityDisplay();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var shipmentItem = button?.DataContext as ShipmentInventoryItem;
            
            if (shipmentItem != null)
            {
                _shipmentItems.Remove(shipmentItem);
                
                // Update the UI
                ShipmentItemsGrid.ItemsSource = null;
                ShipmentItemsGrid.ItemsSource = _shipmentItems;
                
                UpdateCapacityDisplay();
            }
        }

        private void UpdateCapacityDisplay()
        {
            // Ensure the shipment has a truck reference
            if (_shipment.Truck == null)
            {
                var truck = _context.Trucks.Find(_shipment.TruckId);
                if (truck == null)
                {
                    LoadPercentageText.Text = "0%";
                    LoadProgressBar.Value = 0;
                    return;
                }
                _shipment.Truck = truck;
            }

            // Update the shipment's items collection for calculation
            _shipment.ShipmentItems = _shipmentItems;
            
            // Calculate load percentage
            double loadPercentage = _shipmentService.CalculateLoadPercentage(_shipment);
            
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

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Validate truck capacity
            if (!_shipmentService.IsWithinTruckCapacity(_shipment))
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

            // Update the shipment's items collection
            _shipment.ShipmentItems = _shipmentItems;
            
            // For new shipments, the calling window will handle saving
            // For existing shipments, save the changes here
            if (!_isNewShipment)
            {
                // First remove existing items
                var existingItems = _context.ShipmentInventoryItems
                    .Where(si => si.ShipmentId == _shipment.Id)
                    .ToList();
                
                _context.ShipmentInventoryItems.RemoveRange(existingItems);
                
                // Then add the current items
                foreach (var item in _shipmentItems)
                {
                    item.ShipmentId = _shipment.Id;
                    _context.ShipmentInventoryItems.Add(item);
                }
                
                _context.SaveChanges();
            }
            
            this.DialogResult = true;
            this.Close();
        }
    }
}
