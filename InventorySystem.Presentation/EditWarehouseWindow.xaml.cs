using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditWarehouseWindow : Window
    {
        private readonly WarehouseService _warehouseService;
        private readonly Warehouse _warehouse;

        public EditWarehouseWindow(Warehouse warehouse)
        {
            InitializeComponent();
            _warehouse = warehouse;

            // Pre-fill input fields
            NameTextBox.Text = _warehouse.Name;
            LocationTextBox.Text = _warehouse.Location;
            CapacityTextBox.Text = _warehouse.Capacity.ToString();

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

            _warehouse.Name = NameTextBox.Text.Trim();
            _warehouse.Location = LocationTextBox.Text.Trim();
            _warehouse.Capacity = capacity;

            _warehouseService.UpdateWarehouse(_warehouse);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
