using System.Linq;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class TruckMaintenanceWindow : Window
    {
        private readonly TruckService _truckService;
        private readonly AppDbContext _context;

        public TruckMaintenanceWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _truckService = new TruckService(_context);

            LoadTrucks();
        }

        private void LoadTrucks()
        {
            TruckGrid.ItemsSource = _truckService.GetAllTrucks();
        }

        private Truck? SelectedTruck => TruckGrid.SelectedItem as Truck;

        private void AddTruck_Click(object sender, RoutedEventArgs e)
        {
            string identifier = Microsoft.VisualBasic.Interaction.InputBox("Enter truck identifier (e.g. TRK-001):", "Add Truck");
            if (string.IsNullOrWhiteSpace(identifier)) return;

            string capacityStr = Microsoft.VisualBasic.Interaction.InputBox("Enter max capacity in kg:", "Add Truck");
            if (!int.TryParse(capacityStr, out int capacity))
            {
                MessageBox.Show("Invalid capacity.");
                return;
            }

            var truck = new Truck
            {
                Identifier = identifier,
                MaxCapacityKg = capacity,
                Status = "Available"
            };

            _truckService.AddTruck(truck);
            LoadTrucks();
        }

        private void AddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTruck == null)
            {
                MessageBox.Show("Select a truck.");
                return;
            }

            string desc = Microsoft.VisualBasic.Interaction.InputBox("Enter maintenance description:", "New Maintenance");
            if (string.IsNullOrWhiteSpace(desc)) return;

            var record = new TruckMaintenanceRecord
            {
                TruckId = SelectedTruck.Id,
                Description = desc,
                StartDate = DateTime.Now,
                Status = MaintenanceStatus.InProgress
            };

            _truckService.AddMaintenanceRecord(record);
            LoadTrucks();
        }

        private void CompleteMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTruck == null)
            {
                MessageBox.Show("Select a truck.");
                return;
            }

            var activeRecord = _truckService.GetMaintenanceForTruck(SelectedTruck.Id)
                .FirstOrDefault(r => r.Status != MaintenanceStatus.Completed);

            if (activeRecord == null)
            {
                MessageBox.Show("No active maintenance record.");
                return;
            }

            _truckService.CompleteMaintenance(activeRecord.Id);
            LoadTrucks();
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTruck == null)
            {
                MessageBox.Show("Select a truck.");
                return;
            }

            var records = _truckService.GetMaintenanceForTruck(SelectedTruck.Id);
            string summary = string.Join("\n\n", records.Select(r =>
                $"Status: {r.Status}\nDesc: {r.Description}\nStart: {r.StartDate}\nEnd: {r.EndDate?.ToString("g") ?? "Ongoing"}"));

            MessageBox.Show(string.IsNullOrEmpty(summary) ? "No maintenance history." : summary);
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            LoadTrucks();
        }

        private void ShowUnderMaintenance_Click(object sender, RoutedEventArgs e)
        {
            TruckGrid.ItemsSource = _truckService.GetAllTrucks()
                .Where(t => t.IsUnderMaintenance).ToList();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTrucks();
        }
    }
}
