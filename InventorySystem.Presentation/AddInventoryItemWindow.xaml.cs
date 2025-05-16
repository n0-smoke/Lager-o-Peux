using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddInventoryItemWindow : Window
    {
        private readonly InventoryService _inventoryService;

        public AddInventoryItemWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(context);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Basic validation
            if (!int.TryParse(QuantityBox.Text, out int quantity))
            {
                MessageBox.Show("Invalid quantity.");
                return;
            }

            if (!double.TryParse(WeightBox.Text, out double weight))
            {
                MessageBox.Show("Invalid weight per unit.");
                return;
            }

            var item = new InventoryItem
            {
                Name = NameBox.Text,
                Category = CategoryBox.Text,
                Supplier = SupplierBox.Text,
                Quantity = quantity,
                WeightPerUnit = weight,
                CreatedAt = DateTime.UtcNow
            };

            _inventoryService.AddItem(item);

            MessageBox.Show("Item added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}

