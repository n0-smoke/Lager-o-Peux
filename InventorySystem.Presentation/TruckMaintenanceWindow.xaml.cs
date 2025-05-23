using InventorySystem.Domain.Models;
using System;
using System.Linq;
using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class TruckMaintenanceWindow : Window
    {
        private readonly TruckService _truckService;

        public TruckMaintenanceWindow(TruckService truckService)
        {
            InitializeComponent();
            _truckService = truckService;
            RefreshTruckGrid();
        }

        private void RefreshTruckGrid()
        {
            TruckGrid.ItemsSource = null;
            TruckGrid.ItemsSource = _truckService.GetAllTrucks();
        }

        private void AddTruck_Click(object sender, RoutedEventArgs e)
        {
            // Truck Info Pop
            var inputWindow = new TruckInputWindow();
            if (inputWindow.ShowDialog() == true)
            {
                var newTruck = new Truck
                {
                    TruckId = GenerateTruckId(),
                    LicensePlate = inputWindow.LicensePlate,
                    Model = inputWindow.Model,
                    Name = inputWindow.Name,
                    Status = "Available",
                    IsUnderMaintenance = false,
                    MaintenanceRecords = new System.Collections.Generic.List<TruckMaintenanceRecord>()
                };
                _truckService.AddTruck(newTruck);
                RefreshTruckGrid();
                MessageBox.Show("Truck added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int GenerateTruckId()
        {
            var trucks = _truckService.GetAllTrucks();
            return trucks.Any() ? trucks.Max(t => t.TruckId) + 1 : 1;
        }

        private void UpdateTruckStatus_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck first.", "No Truck Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var statusWindow = new TruckStatusWindow(selectedTruck.Status);
            if (statusWindow.ShowDialog() == true)
            {
                _truckService.UpdateTruckStatus(selectedTruck, statusWindow.NewStatus);
                RefreshTruckGrid();
                MessageBox.Show($"Truck status updated to {statusWindow.NewStatus}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is not Truck selectedTruck)
            {
                MessageBox.Show("Please select a truck first.", "No Truck Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var maintenanceWindow = new MaintenanceInputWindow();
            if (maintenanceWindow.ShowDialog() == true)
            {
                var record = new TruckMaintenanceRecord
                {
                    Id = GenerateMaintenanceId(),
                    TruckId = selectedTruck.TruckId,
                    Description = maintenanceWindow.Description,
                    StartDate = DateTime.Now,
                    Status = MaintenanceStatus.InProgress
                };

                _truckService.AddMaintenanceRecord(record);
                RefreshTruckGrid();
                MessageBox.Show("Maintenance record added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int GenerateMaintenanceId()
        {
            var allRecords = _truckService.GetAllMaintenanceRecords();
            return allRecords.Any() ? allRecords.Max(r => r.Id) + 1 : 1;
        }

        private void ViewRecords_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is Truck selectedTruck)
            {
                var records = _truckService.GetMaintenanceForTruck(selectedTruck.TruckId);
                string message = string.Join("\n", records.Select(r =>
                    $"{r.Description} - {r.Status} ({r.StartDate.ToShortDateString()} - {(r.EndDate?.ToShortDateString() ?? "Now")})"));
                MessageBox.Show(message, "Maintenance Records");
            }
        }

        private void CompleteMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is Truck selectedTruck)
            {
                var records = _truckService.GetMaintenanceForTruck(selectedTruck.TruckId)
                    .Where(r => r.Status != MaintenanceStatus.Completed)
                    .ToList();

                if (records.Count > 0)
                {
                    var latest = records.OrderByDescending(r => r.StartDate).First();
                    _truckService.CompleteMaintenance(latest.Id);
                    RefreshTruckGrid();
                    MessageBox.Show("Maintenance completed and truck status updated.", "Success");
                }
                else
                {
                    MessageBox.Show("No ongoing maintenance for this truck.");
                }
            }
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            RefreshTruckGrid();
        }

        private void ShowOngoing_Click(object sender, RoutedEventArgs e)
        {
            TruckGrid.ItemsSource = _truckService.GetAllTrucks()
                .Where(t => t.IsUnderMaintenance).ToList();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
