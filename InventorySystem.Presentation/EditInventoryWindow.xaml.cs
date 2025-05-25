using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditInventoryWindow : Window
    {
        private readonly InventoryService _inventoryService;
        private readonly InventoryItem _item;

        public EditInventoryWindow(InventoryItem item)
        {
            InitializeComponent();
            _item = item;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(context);

            // Pre-fill form
            NameBox.Text = _item.Name;
            CategoryBox.Text = _item.Category;
            WeightBox.Text = _item.WeightPerUnit.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(WeightBox.Text, out double weight))
            {
                MessageBox.Show("Weight must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _item.Name = NameBox.Text.Trim();
            _item.Category = CategoryBox.Text.Trim();
            _item.WeightPerUnit = weight;

            _inventoryService.UpdateItem(_item);

            MessageBox.Show("Inventory item updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}
