using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class WarehouseWindow : Window
    {
        private readonly WarehouseService _warehouseService;
        private readonly AppDbContext _context;

        public WarehouseWindow()
        {
            InitializeComponent();

            // Configure DbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _warehouseService = new WarehouseService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var warehouses = _warehouseService.GetAllWarehouses();
            WarehouseGrid.ItemsSource = warehouses;
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
