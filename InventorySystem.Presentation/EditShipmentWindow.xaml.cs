using System;
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
            LoadShipmentItems();

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
            
            // Set up event handlers
            TruckComboBox.SelectionChanged += (s, e) => UpdateCapacityDisplay();
            
            // Initial capacity display
            UpdateCapacityDisplay();
        }

        private void LoadTrucks()
        {
            var trucks = _context.Trucks.ToList();
            TruckComboBox.ItemsSource = trucks;
        }
        
        private void LoadShipmentItems()
        {
            // Load shipment items with their related inventory items
            var shipmentWithItems = _context.Shipments
                .Include(s => s.ShipmentItems)
                .ThenInclude(si => si.InventoryItem)
                .FirstOrDefault(s => s.Id == _shipment.Id);

            if (shipmentWithItems != null && shipmentWithItems.ShipmentItems != null)
            {
                _shipment.ShipmentItems = shipmentWithItems.ShipmentItems;
                ShipmentItemsGrid.ItemsSource = _shipment.ShipmentItems;
            }
        }
        
        private void ManageItems_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck before managing shipment items.");
                return;
            }

            // Update the shipment's truck reference
            _shipment.TruckId = selectedTruck.Id;
            _shipment.Truck = selectedTruck;

            // Open the manage shipment items window
            var manageItemsWindow = new ManageShipmentItemsWindow(_shipment);
            bool? result = manageItemsWindow.ShowDialog();

            if (result == true)
            {
                // Reload shipment items
                LoadShipmentItems();
                
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
            _shipment.TruckId = selectedTruck.Id;
            _shipment.Truck = selectedTruck;
            
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            _shipment.TruckId = selectedTruck.Id;
            _shipment.Truck = selectedTruck; // Ensure truck reference is set
            _shipment.Destination = DestinationBox.Text;
            _shipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";

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

            _shipmentService.UpdateShipment(_shipment);

            MessageBox.Show("Shipment updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
