using System.Collections.Generic;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class InventoryWindow : Window
    {
        private readonly InventoryService _inventoryService;

        public InventoryWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(context);

            LoadInventory();
        }

        private void LoadInventory()
        {
            List<InventoryItem> items = _inventoryService.GetAllItems();
            InventoryGrid.ItemsSource = items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddInventoryWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadInventory();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryGrid.SelectedItem is InventoryItem selectedItem)
            {
                var editWindow = new EditInventoryWindow(selectedItem);
                if (editWindow.ShowDialog() == true)
                {
                    LoadInventory();
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

                if (result == MessageBoxResult.Yes)
                {
                    _inventoryService.DeleteItem(selectedItem.Id);
                    LoadInventory();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }
    }
}
