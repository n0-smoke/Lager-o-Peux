using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using InventorySystem.Presentation.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventorySystem.Presentation
{
    public partial class AddShipmentWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly int? editingShipmentId = null;
        private Shipment editingShipment;
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
        public AddShipmentWindow(int shipmentId) : this()
        {
            editingShipmentId = shipmentId;
            LoadShipmentForEditing(shipmentId);
        }


        private void LoadInventoryItems()
        {
            var all = _context.InventoryItems.ToList();
            var viewModels = all.Select(i => new InventoryItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Category = i.Category,
                WeightPerUnit = i.WeightPerUnit
            }).ToList();

            InventorySelectionGrid.ItemsSource = viewModels;
        }

        private void LoadShipmentForEditing(int shipmentId)
        {
            editingShipment = _context.Shipments
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                .Include(s => s.Truck)
                .Include(s => s.Route)
                .FirstOrDefault(s => s.Id == shipmentId);

            if (editingShipment == null)
            {
                MessageBox.Show("Shipment not found.");
                Close();
                return;
            }

            // Load trucks for route first
            var currentLocation = SessionManager.CurrentUser?.Location ?? "Sarajevo";
            string destination = editingShipment.Route.Location1 == currentLocation
                ? editingShipment.Route.Location2
                : editingShipment.Route.Location1;

            // Set destination selection
            var destinationIsWarehouse = _context.Warehouses.Any(w => w.Location == destination);
            DestinationTypeComboBox.SelectedIndex = destinationIsWarehouse ? 0 : 1;
            DestinationEntityComboBox.ItemsSource = destinationIsWarehouse
                ? _context.Warehouses.Where(w => w.Location != currentLocation).ToList()
                : _context.Clients.Where(c => c.Location != currentLocation).ToList();
            DestinationEntityComboBox.DisplayMemberPath = "Name";
            DestinationEntityComboBox.SelectedItem = destinationIsWarehouse
                ? _context.Warehouses.FirstOrDefault(w => w.Location == destination)
                : _context.Clients.FirstOrDefault(c => c.Location == destination);

            // Reload truck options for the route
            trucks = _context.Trucks
                .Where(t => t.RouteId == editingShipment.RouteId && t.Location == currentLocation)
                .ToList();
            TruckComboBox.ItemsSource = trucks.Select(t => new { Truck = t, Label = $"{t.Name} ({t.LoadCapacity} kg)" }).ToList();
            TruckComboBox.DisplayMemberPath = "Label";
            TruckComboBox.SelectedValuePath = "Truck";
            TruckComboBox.SelectedIndex = trucks.FindIndex(t => t.Id == editingShipment.TruckId);

            // Load shipment items
            shipmentItems = editingShipment.ShipmentItems.Select(si => new ShipmentItemViewModel
            {
                ItemName = si.InventoryItem.Name,
                Quantity = si.Amount,
                Weight = si.Amount * si.InventoryItem.WeightPerUnit
            }).ToList();
            ShipmentItemsGrid.ItemsSource = null;
            ShipmentItemsGrid.ItemsSource = shipmentItems;

            UpdateTruckLoadDisplay();
        }

        private void RemoveShipmentItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ShipmentItemViewModel item)
            {
                shipmentItems.Remove(item);
                ShipmentItemsGrid.ItemsSource = null;
                ShipmentItemsGrid.ItemsSource = shipmentItems;
                UpdateTruckLoadDisplay();
            }
        }

        private void AddInventoryRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is InventoryItemViewModel row)
            {
                // 👇 Force the DataGrid to update the binding from edited cell
                InventorySelectionGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                InventorySelectionGrid.CommitEdit(DataGridEditingUnit.Row, true);

                if (row.InputQuantity <= 0)
                {
                    MessageBox.Show("Please enter a quantity greater than 0.");
                    return;
                }

                double weight = row.InputQuantity * row.WeightPerUnit;

                shipmentItems.Add(new ShipmentItemViewModel
                {
                    ItemName = row.Name,
                    Quantity = row.InputQuantity,
                    Weight = weight
                });

                ShipmentItemsGrid.ItemsSource = null;
                ShipmentItemsGrid.ItemsSource = shipmentItems;

                UpdateTruckLoadDisplay();
                row.InputQuantity = 0;
                InventorySelectionGrid.Items.Refresh();
            }
        }

        private void DestinationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DestinationTypeComboBox.SelectedItem is ComboBoxItem selected)
            {
                string currentLocation = SessionManager.CurrentUser?.Location ?? "Sarajevo";

                if (selected.Content.ToString() == "Warehouse")
                {
                    DestinationEntityComboBox.ItemsSource = _context.Warehouses
                        .Where(w => w.Location != currentLocation) // ✅ Exclude current location
                        .ToList();
                    DestinationEntityComboBox.DisplayMemberPath = "Name";
                }
                else
                {
                    DestinationEntityComboBox.ItemsSource = _context.Clients
                        .Where(c => c.Location != currentLocation) // ✅ Exclude current location
                        .ToList();
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

            // ✅ Defensive guard for same-location edge case
            if (destinationLocation == currentLocation)
            {
                MessageBox.Show("Cannot send a shipment to the current location.");
                TruckComboBox.ItemsSource = null;
                TruckLoadText.Text = "0 / 0 kg";
                TruckLoadProgressBar.Value = 0;
                return;
            }

            var route = _context.Routes.FirstOrDefault(r => r.Location1 == currentLocation && r.Location2 == destinationLocation);
            if (route == null)
            {
                MessageBox.Show("No route found between current location and destination.");
                return;
            }

            trucks = _context.Trucks
                .Where(t => t.RouteId == route.Id && t.Location == currentLocation && t.Availability)
                .ToList();

            // ✅ Include truck label with capacity
            TruckComboBox.ItemsSource = trucks.Select(t => new { Truck = t, Label = $"{t.Name} ({t.LoadCapacity} kg)" }).ToList();
            TruckComboBox.DisplayMemberPath = "Label";
            TruckComboBox.SelectedValuePath = "Truck";
            TruckComboBox.SelectedIndex = 0;
        }

        private void TruckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTruckLoadDisplay();
        }

        private void UpdateTruckLoadDisplay()
        {
            if (TruckComboBox.SelectedItem == null) return;

            var selected = TruckComboBox.SelectedItem;
            Truck selectedTruck = selected.GetType().GetProperty("Truck")?.GetValue(selected) as Truck;

            if (selectedTruck == null) return;

            // Load existing shipment weight from DB
            var pastWeight = _context.Shipments
                .Where(s => s.TruckId == selectedTruck.Id)
                .SelectMany(s => s.ShipmentItems)
                .Join(_context.InventoryItems,
                      si => si.InventoryItemId,
                      ii => ii.Id,
                      (si, ii) => si.Amount * ii.WeightPerUnit)
                .Sum();

            // Calculate new weight from current UI
            var draftWeight = shipmentItems.Sum(i => i.Weight);
            var totalWeight = pastWeight + draftWeight;

            TruckLoadText.Text = $"{totalWeight} / {selectedTruck.LoadCapacity} kg";
            TruckLoadProgressBar.Value = Math.Min(100, (totalWeight / selectedTruck.LoadCapacity) * 100);
        }


        private async void SubmitShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (inventoryItems == null || inventoryItems.Count == 0)
            {
                inventoryItems = _context.InventoryItems.ToList();
            }

            if (TruckComboBox.SelectedItem == null || DestinationEntityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select both a destination and a truck.");
                return;
            }

            if (shipmentItems.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the shipment.");
                return;
            }

            var selectedTruck = TruckComboBox.SelectedItem.GetType().GetProperty("Truck")?.GetValue(TruckComboBox.SelectedItem) as Truck;
            if (selectedTruck == null)
            {
                MessageBox.Show("Truck selection invalid.");
                return;
            }

            int destinationEntityId = DestinationTypeComboBox.Text == "Warehouse"
                ? ((Warehouse)DestinationEntityComboBox.SelectedItem).Id
                : ((Client)DestinationEntityComboBox.SelectedItem).Id;


            try
            {
                var service = new ShipmentService(_context);

                // Build DTO safely
                var itemDtos = shipmentItems.Select(s =>
                {
                    var item = inventoryItems.FirstOrDefault(i => i.Name == s.ItemName);
                    if (item == null)
                        throw new InvalidOperationException($"Item '{s.ItemName}' was not found in inventory.");

                    return new ShipmentItemCreateDto
                    {
                        InventoryItemId = item.Id,
                        Amount = s.Quantity
                    };
                }).ToList();

                Console.WriteLine($"Items to ship: {shipmentItems.Count}");
                foreach (var s in shipmentItems)
                {
                    Console.WriteLine($"Item: {s.ItemName}, Qty: {s.Quantity}");
                }

                Console.WriteLine($"DEBUG: DestinationType = {DestinationTypeComboBox.Text}");
                Console.WriteLine($"DEBUG: DestinationEntityId = {destinationEntityId}");
                Console.WriteLine($"DEBUG: TruckId = {selectedTruck.Id}");

                foreach (var s in shipmentItems)
                {
                    var match = inventoryItems.FirstOrDefault(i => i.Name == s.ItemName);
                    if (match == null)
                    {
                        MessageBox.Show($"ERROR: Could not find item with name: {s.ItemName}");
                        return;
                    }

                    Console.WriteLine($"DEBUG ITEM: {s.ItemName}, Qty = {s.Quantity}, Mapped ID = {match.Id}");
                }

                if (editingShipmentId.HasValue)
                {
                    await service.UpdateShipmentAsync(editingShipmentId.Value, new ShipmentCreateDto
                    {
                        DestinationType = DestinationTypeComboBox.Text,
                        DestinationEntityId = destinationEntityId,
                        TruckId = selectedTruck.Id,
                        Items = itemDtos
                    }, SessionManager.CurrentUser?.Location ?? "Sarajevo");

                    MessageBox.Show("Shipment updated successfully.");
                }
                else
                {
                    await service.CreateShipmentAsync(new ShipmentCreateDto
                    {
                        DestinationType = DestinationTypeComboBox.Text,
                        DestinationEntityId = destinationEntityId,
                        TruckId = selectedTruck.Id,
                        Items = itemDtos
                    }, SessionManager.CurrentUser?.Location ?? "Sarajevo");

                    MessageBox.Show("Shipment created successfully.");
                }

                MessageBox.Show("Shipment successfully submitted.");
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: DestinationType = {DestinationTypeComboBox.Text}");
                Console.WriteLine($"DEBUG: DestinationEntityId = {destinationEntityId}");
                Console.WriteLine($"DEBUG: TruckId = {selectedTruck.Id}");

                foreach (var s in shipmentItems)
                {
                    var match = inventoryItems.FirstOrDefault(i => i.Name == s.ItemName);
                    if (match == null)
                    {
                        MessageBox.Show($"ERROR: Could not find item with name: {s.ItemName}");
                        return;
                    }

                    Console.WriteLine($"DEBUG ITEM: {s.ItemName}, Qty = {s.Quantity}, Mapped ID = {match.Id}");
                }

                MessageBox.Show($"Shipment failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class ShipmentItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
    }
    public class InventoryItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double WeightPerUnit { get; set; }
        public int InputQuantity { get; set; } = 0;
    }

    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 &&
                double.TryParse(values[0]?.ToString(), out double weight) &&
                int.TryParse(values[1]?.ToString(), out int qty))
            {
                return (weight * qty).ToString("F2");
            }
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
