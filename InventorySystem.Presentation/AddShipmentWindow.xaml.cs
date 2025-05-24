using InventorySystem.Application.DTOs;
using InventorySystem.Application.Services;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
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
        private readonly IShipmentService _shipmentService;
        private readonly string _currentUserLocation = "Sarajevo"; // Placeholder for logged-in user

        private List<ShipmentItemDisplay> _shipmentItems = new();
        private Route _resolvedRoute;

        public AddShipmentWindow()
        {
            InitializeComponent();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True")
                .Options;

            _context = new AppDbContext(options);
            _shipmentService = new ShipmentService(_context);

            LoadInventoryItems();
        }

        private void LoadInventoryItems()
        {
            var items = _context.InventoryItems.ToList();
            InventoryItemComboBox.ItemsSource = items;
            InventoryItemComboBox.DisplayMemberPath = "Name";
            InventoryItemComboBox.SelectedValuePath = "Id";
        }

        private void DestinationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DestinationTypeComboBox.SelectedItem is ComboBoxItem selected)
            {
                string type = selected.Content.ToString();

                if (type == "Client")
                {
                    var clients = _context.Clients.ToList();
                    DestinationEntityComboBox.ItemsSource = clients;
                    DestinationEntityComboBox.DisplayMemberPath = "Name";
                    DestinationEntityComboBox.SelectedValuePath = "Id";
                }
                else if (type == "Warehouse")
                {
                    var warehouses = _context.Warehouses.ToList();
                    DestinationEntityComboBox.ItemsSource = warehouses;
                    DestinationEntityComboBox.DisplayMemberPath = "Name";
                    DestinationEntityComboBox.SelectedValuePath = "Id";
                }
            }

            DestinationEntityComboBox.SelectionChanged += DestinationEntityComboBox_SelectionChanged;
        }

        private void DestinationEntityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResolveRouteAndLoadTrucks();
        }

        private void ResolveRouteAndLoadTrucks()
        {
            if (DestinationEntityComboBox.SelectedItem == null || DestinationTypeComboBox.SelectedItem is not ComboBoxItem selectedType)
                return;

            string destinationLocation = string.Empty;

            if (selectedType.Content.ToString() == "Client" && DestinationEntityComboBox.SelectedItem is Client client)
                destinationLocation = client.Location;
            else if (selectedType.Content.ToString() == "Warehouse" && DestinationEntityComboBox.SelectedItem is Warehouse warehouse)
                destinationLocation = warehouse.Location;
            else
                return;

            _resolvedRoute = _context.Routes
                .FirstOrDefault(r => r.Location1 == _currentUserLocation && r.Location2 == destinationLocation)
                ?? _context.Routes.FirstOrDefault(r => r.Location2 == _currentUserLocation && r.Location1 == destinationLocation);

            if (_resolvedRoute == null)
            {
                MessageBox.Show("No route found between current location and destination.");
                return;
            }

            var trucks = _context.Trucks
                .Where(t => t.RouteId == _resolvedRoute.Id && t.Location == _currentUserLocation && t.Availability)
                .ToList();

            if (!trucks.Any())
            {
                MessageBox.Show("No available trucks for this route.");
            }

            TruckComboBox.ItemsSource = trucks;
            TruckComboBox.DisplayMemberPath = "Name";
            TruckComboBox.SelectedValuePath = "Id";
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryItemComboBox.SelectedItem is InventoryItem item && int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                var weight = item.WeightPerUnit * quantity;

                _shipmentItems.Add(new ShipmentItemDisplay
                {
                    ItemName = item.Name,
                    Quantity = quantity,
                    Weight = weight,
                    InventoryItemId = item.Id
                });

                ShipmentItemsGrid.ItemsSource = null;
                ShipmentItemsGrid.ItemsSource = _shipmentItems;
            }
        }

        private async void SubmitShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_resolvedRoute == null)
            {
                MessageBox.Show("Please select a valid destination and route first.");
                return;
            }

            if (TruckComboBox.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck.");
                return;
            }

            if (_shipmentItems.Count == 0)
            {
                MessageBox.Show("Please add at least one inventory item.");
                return;
            }

            string destinationType = (DestinationTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int destinationId = (int)(DestinationEntityComboBox.SelectedValue ?? -1);

            var shipmentDto = new ShipmentCreateDto
            {
                DestinationType = destinationType,
                DestinationEntityId = destinationId,
                TruckId = selectedTruck.Id,
                Items = _shipmentItems.Select(i => new ShipmentItemCreateDto
                {
                    InventoryItemId = i.InventoryItemId,
                    Amount = i.Quantity
                }).ToList()
            };

            try
            {
                await _shipmentService.CreateShipmentAsync(shipmentDto, _currentUserLocation);
                MessageBox.Show("Shipment successfully created!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shipment: {ex.Message}");
            }
        }

        private class ShipmentItemDisplay
        {
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public double Weight { get; set; }
            public int InventoryItemId { get; set; }
        }
    }
}
