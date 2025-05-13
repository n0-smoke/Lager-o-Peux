using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class ShipmentWindow : Window
    {
        private readonly ShipmentService _shipmentService;
        private readonly AppDbContext _context;

        public ShipmentWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _shipmentService = new ShipmentService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var shipments = _shipmentService.GetAllShipments();
            ShipmentGrid.ItemsSource = shipments;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddShipmentWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = ShipmentGrid.SelectedItem as Shipment;

            if (selected == null)
            {
                MessageBox.Show("Please select a shipment to edit.");
                return;
            }

            var editWindow = new EditShipmentWindow(selected);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = ShipmentGrid.SelectedItem as Shipment;

            if (selected == null)
            {
                MessageBox.Show("Please select a shipment to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete shipment to '{selected.Destination}'?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                _shipmentService.DeleteShipment(selected.Id);
                LoadData();
                MessageBox.Show("Shipment deleted.");
            }
        }

    }
}

