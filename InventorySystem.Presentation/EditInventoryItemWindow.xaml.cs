using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditInventoryItemWindow : Window
    {
        private readonly InventoryService _inventoryService;
        private readonly InventoryItem _item;

        public EditInventoryItemWindow(InventoryItem item)
        {
            InitializeComponent();
            _item = item;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(context);

            // Populate UI with current item data
            NameBox.Text = _item.Name;
            CategoryBox.Text = _item.Category;
            QuantityBox.Text = _item.Quantity.ToString();
            SupplierBox.Text = _item.Supplier;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _item.Name = NameBox.Text;
            _item.Category = CategoryBox.Text;
            _item.Quantity = int.TryParse(QuantityBox.Text, out int quantity) ? quantity : 0;
            _item.Supplier = SupplierBox.Text;

            _inventoryService.UpdateItem(_item);

            MessageBox.Show("Item updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
