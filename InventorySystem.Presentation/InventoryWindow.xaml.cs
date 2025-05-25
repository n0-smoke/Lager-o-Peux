using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class InventoryWindow : Window
    {
        private InventoryService? _inventoryService;

        public InventoryWindow()
        {
            InitializeComponent();

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

                var context = new AppDbContext(optionsBuilder.Options);
                _inventoryService = new InventoryService(context);

                LoadInventory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize inventory service: " + ex.Message);
            }
        }

        private void LoadInventory(string? categoryFilter = null)
        {
            if (_inventoryService == null)
            {
                MessageBox.Show("Inventory service is not available.");
                return;
            }

            try
            {
                List<InventoryItem> items = _inventoryService.GetAllItems();

                if (!string.IsNullOrWhiteSpace(categoryFilter))
                {
                    items = items
                        .Where(i => i.Category != null &&
                                    i.Category.Contains(categoryFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                InventoryGrid.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddInventoryWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadInventory(CategorySearchBox.Text.Trim());
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is InventoryItem selectedItem)
            {
                var editWindow = new EditInventoryWindow(selectedItem);
                if (editWindow.ShowDialog() == true)
                {
                    LoadInventory(CategorySearchBox.Text.Trim());
                }
            }
            else
            {
                MessageBox.Show("Please select an item to edit.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is InventoryItem selectedItem)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedItem.Name}'?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes && _inventoryService != null)
                {
                    _inventoryService.DeleteItem(selectedItem.Id);
                    LoadInventory(CategorySearchBox.Text.Trim());
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void CategorySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadInventory(CategorySearchBox.Text.Trim());
        }
    }
}