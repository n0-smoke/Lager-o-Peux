using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class TruckWindow : Window
    {
        private readonly TruckService _truckService;
        private readonly WeatherService _weatherService = new();

        public TruckWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            var context = new AppDbContext(optionsBuilder.Options);
            _truckService = new TruckService(context);

            LoadTrucks();
        }

        private void LoadTrucks()
        {
            List<Truck> trucks = _truckService.GetAllTrucks();
            TruckGrid.ItemsSource = trucks;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTruckWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadTrucks();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is Truck selectedTruck)
            {
                var editWindow = new EditTruckWindow(selectedTruck);
                if (editWindow.ShowDialog() == true)
                {
                    LoadTrucks();
                }
            }
            else
            {
                MessageBox.Show("Please select a truck to edit.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (TruckGrid.SelectedItem is Truck selectedTruck)
            {
                var result = MessageBox.Show($"Are you sure you want to delete truck '{selectedTruck.Name}'?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _truckService.DeleteTruck(selectedTruck.Id);
                    LoadTrucks();
                }
            }
            else
            {
                MessageBox.Show("Please select a truck to delete.");
            }
        }

        private async void TruckGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TruckGrid.SelectedItem is Truck selectedTruck)
            {
                string location = selectedTruck.Location;

                if (!string.IsNullOrWhiteSpace(location))
                {
                    WeatherTextBlock.Text = "Loading weather for " + location + "...";
                    string weather = await _weatherService.GetCurrentWeatherAsync(location);
                    WeatherTextBlock.Text = weather;
                }
                else
                {
                    WeatherTextBlock.Text = "No location set for this truck.";
                }
            }
            else
            {
                WeatherTextBlock.Text = string.Empty;
            }
        }
    }
}
