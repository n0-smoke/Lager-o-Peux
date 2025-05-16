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
        private readonly AppDbContext _context;

        public InventoryWindow()
        {
            InitializeComponent();

            // Configure DbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _inventoryService = new InventoryService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var items = _inventoryService.GetAllItems();
            InventoryGrid.ItemsSource = items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddInventoryItemWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = InventoryGrid.SelectedItem as InventoryItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item to edit.");
                return;
            }

            var editWindow = new EditInventoryItemWindow(selectedItem);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = InventoryGrid.SelectedItem as InventoryItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{selectedItem.Name}'?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _inventoryService.DeleteItem(selectedItem.Id);
                LoadData();
                MessageBox.Show("Item deleted.");
            }
        }
    }
}
