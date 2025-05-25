using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddInventoryWindow : Window
    {
        private readonly InventoryService _inventoryService;

        public AddInventoryWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(context);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(WeightBox.Text, out double weight))
            {
                MessageBox.Show("Weight must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = new InventoryItem
            {
                Name = NameBox.Text.Trim(),
                Category = CategoryBox.Text.Trim(),
                WeightPerUnit = weight
            };

            _inventoryService.AddItem(item);

            MessageBox.Show("Inventory item added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}
