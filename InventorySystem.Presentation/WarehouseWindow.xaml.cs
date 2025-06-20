﻿using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class WarehouseWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly WarehouseService _warehouseService;
        private readonly WeatherService _weatherService = new();

        public WarehouseWindow()
        {
            InitializeComponent();

            // Configure DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True")
                .Options;

            _context = new AppDbContext(options);
            _warehouseService = new WarehouseService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            WarehouseGrid.ItemsSource = warehouses;
        }

        private async void WarehouseGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarehouseGrid.SelectedItem is Warehouse selectedWarehouse)
            {
                string location = selectedWarehouse.Location;

                if (!string.IsNullOrWhiteSpace(location))
                {
                    WeatherTextBlock.Text = "Loading weather for " + location + "...";
                    string weather = await _weatherService.GetCurrentWeatherAsync(location);
                    WeatherTextBlock.Text = weather;
                }
                else
                {
                    WeatherTextBlock.Text = "No location set for this warehouse.";
                }
            }
            else
            {
                WeatherTextBlock.Text = string.Empty;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWarehouseWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedWarehouse = WarehouseGrid.SelectedItem as Warehouse;

            if (selectedWarehouse == null)
            {
                MessageBox.Show("Please select a warehouse to edit.");
                return;
            }

            var editWindow = new EditWarehouseWindow(selectedWarehouse);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedWarehouse = WarehouseGrid.SelectedItem as Warehouse;

            if (selectedWarehouse == null)
            {
                MessageBox.Show("Please select a warehouse to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{selectedWarehouse.Name}'?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _warehouseService.DeleteWarehouse(selectedWarehouse.Id);
                LoadData();
                MessageBox.Show("Warehouse deleted.");
            }
        }
    }
}
