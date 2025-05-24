using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Diagnostics;

namespace InventorySystem.Presentation
{
    public partial class EditShipmentWindow : Window
    {
        private readonly ShipmentService _shipmentService;
        private readonly AppDbContext _context;
        private readonly Shipment _shipment;
        private List<ShipmentInventoryItem> assignedItems = new();

        public EditShipmentWindow(Shipment shipment)
        {
            InitializeComponent();
            _shipment = shipment;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);

            _context.Entry(_shipment).Collection(s => s.ShipmentItems)
                .Query()
                .Include(i => i.InventoryItem)
                .Load();

            assignedItems = _shipment.ShipmentItems.ToList();

            LoadTrucks();
            PopulateForm();

            TruckComboBox.SelectionChanged += TruckComboBox_SelectionChanged;
        }

        private void LoadTrucks()
        {
            var trucks = _context.Trucks.ToList();
            TruckComboBox.ItemsSource = trucks;
        }

        private void PopulateForm()
        {
            DestinationBox.Text = _shipment.Destination;
            TruckComboBox.SelectedValue = _shipment.TruckId;

            foreach (ComboBoxItem item in DirectionBox.Items)
            {
                if (item.Content?.ToString() == _shipment.Direction.ToString())
                {
                    item.IsSelected = true;
                    break;
                }
            }

            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Content?.ToString() == _shipment.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            UpdateLoadUI();
        }

        private void AssignItems_Click(object sender, RoutedEventArgs e)
        {
            var assignWindow = new AssignItemsWindow();
            bool? result = assignWindow.ShowDialog();

            if (result == true)
            {
                assignedItems = assignWindow.SelectedItems;
                MessageBox.Show($"{assignedItems.Count} item(s) assigned.");
                UpdateLoadUI();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            var directionString = ((ComboBoxItem)DirectionBox.SelectedItem)?.Content?.ToString();
            var parsed = Enum.TryParse<ShipmentDirection>(directionString, out var shipmentDirection);
            if (!parsed)
            {
                MessageBox.Show("Please select a shipment direction.");
                return;
            }

            var simulatedShipment = new Shipment
            {
                TruckId = selectedTruck.Id,
                Truck = selectedTruck,
                ShipmentItems = assignedItems
            };

            if (!_shipmentService.IsWithinTruckCapacity(simulatedShipment))
            {
                var confirm = MessageBox.Show(
                    "Warning: Shipment exceeds truck capacity. Proceed anyway?",
                    "Overload Warning",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (confirm != MessageBoxResult.Yes)
                    return;
            }

            _shipment.TruckId = selectedTruck.Id;
            _shipment.Destination = DestinationBox.Text;
            _shipment.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";
            _shipment.Direction = shipmentDirection;
            _shipment.ShipmentItems = assignedItems;

            if (_shipment.Status == "Delivered" && !_shipment.IsInventoryApplied)
            {
                try
                {
                    _shipmentService.ApplyShipmentToInventory(_shipment);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Inventory Adjustment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _shipmentService.UpdateShipment(_shipment);
            MessageBox.Show("Shipment updated successfully.");
            DialogResult = true;
            Close();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (_shipment == null)
            {
                MessageBox.Show("No shipment selected.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Save Shipment Report",
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Shipment_{_shipment.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var reportService = new ReportService(_context, _shipmentService);
                    string fullPath = reportService.GenerateShipmentReport(_shipment.Id,
                        Path.GetDirectoryName(dialog.FileName), Environment.UserName);

                    // Optional: open PDF automatically
                    if (File.Exists(fullPath))
                    {
                        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to generate report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TruckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLoadUI();
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

            var simulatedShipment = new Shipment
            {
                TruckId = selectedTruck.Id,
                Truck = selectedTruck,
                ShipmentItems = assignedItems
            };

            double load = _shipmentService.CalculateLoadPercentage(simulatedShipment);
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
    }
}
