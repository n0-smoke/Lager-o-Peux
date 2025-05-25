using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddTruckWindow : Window
    {
        private readonly TruckService _truckService;

        public AddTruckWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _truckService = new TruckService(context);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(RouteIdBox.Text, out int routeId) ||
                !int.TryParse(DriverIdBox.Text, out int driverId) ||
                !double.TryParse(CapacityBox.Text, out double capacity) ||
                !double.TryParse(FuelBox.Text, out double fuel) ||
                !double.TryParse(MileageBox.Text, out double mileage))
            {
                MessageBox.Show("Please check that numeric fields are valid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var truck = new Truck
            {
                Name = NameBox.Text,
                RouteId = routeId,
                DriverId = driverId,
                LoadCapacity = capacity,
                Location = LocationBox.Text,
                FuelConsumption = fuel,
                Mileage = mileage,
                Availability = AvailabilityBox.IsChecked ?? false
            };

            _truckService.AddTruck(truck);

            MessageBox.Show("Truck added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}