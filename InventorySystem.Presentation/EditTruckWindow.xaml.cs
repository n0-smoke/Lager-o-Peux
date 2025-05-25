using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditTruckWindow : Window
    {
        private readonly TruckService _truckService;
        private readonly Truck _truck;

        public EditTruckWindow(Truck truck)
        {
            InitializeComponent();
            _truck = truck;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _truckService = new TruckService(context);

            // Fill the form
            NameBox.Text = _truck.Name;
            RouteIdBox.Text = _truck.RouteId.ToString();
            DriverIdBox.Text = _truck.DriverId.ToString();
            CapacityBox.Text = _truck.LoadCapacity.ToString();
            LocationBox.Text = _truck.Location;
            FuelBox.Text = _truck.FuelConsumption.ToString();
            MileageBox.Text = _truck.Mileage.ToString();
            AvailabilityBox.IsChecked = _truck.Availability;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(RouteIdBox.Text, out int routeId) ||
                !int.TryParse(DriverIdBox.Text, out int driverId) ||
                !double.TryParse(CapacityBox.Text, out double capacity) ||
                !double.TryParse(FuelBox.Text, out double fuel) ||
                !double.TryParse(MileageBox.Text, out double mileage))
            {
                MessageBox.Show("Please enter valid numeric values.");
                return;
            }

            _truck.Name = NameBox.Text;
            _truck.RouteId = routeId;
            _truck.DriverId = driverId;
            _truck.LoadCapacity = capacity;
            _truck.Location = LocationBox.Text;
            _truck.FuelConsumption = fuel;
            _truck.Mileage = mileage;
            _truck.Availability = AvailabilityBox.IsChecked ?? false;

            _truckService.UpdateTruck(_truck);

            MessageBox.Show("Truck updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}