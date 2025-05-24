using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddWarehouseWindow : Window
    {
        private readonly WarehouseService _warehouseService;

        public AddWarehouseWindow()
        {
            InitializeComponent();

            // Setup DB Context and Service
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _warehouseService = new WarehouseService(context);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LocationTextBox.Text) ||
                !int.TryParse(CapacityTextBox.Text, out int capacity))
            {
                MessageBox.Show("Please enter valid warehouse details.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var warehouse = new Warehouse
            {
                Name = NameTextBox.Text.Trim(),
                Location = LocationTextBox.Text.Trim(),
                Capacity = capacity
            };

            _warehouseService.AddWarehouse(warehouse);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}