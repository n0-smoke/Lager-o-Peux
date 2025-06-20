﻿using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InventorySystem.Presentation
{
    public partial class ShipmentOverviewWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly WeatherService _weatherService = new();

        public ShipmentOverviewWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");
            _context = new AppDbContext(optionsBuilder.Options);

            LoadShipments();
        }

        private void LoadShipments()
        {
            var shipments = _context.Shipments
                .Include(s => s.Truck)
                .Include(s => s.Route)
                .Include(s => s.ShipmentItems)
                    .ThenInclude(si => si.InventoryItem)
                .ToList()
                .Select(s => new ShipmentOverviewViewModel
                {
                    Id = s.Id,
                    Truck = s.Truck,
                    RouteDisplay = $"{s.Route.Location1} ➜ {s.Route.Location2}",
                    TotalWeight = s.ShipmentItems.Sum(si => si.Amount * si.InventoryItem.WeightPerUnit)
                }).ToList();

            ShipmentsGrid.ItemsSource = shipments;
        }

        private void AddShipment_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddShipmentWindow();
            addWindow.ShowDialog();
            LoadShipments();
        }

        private void EditShipment_Click(object sender, RoutedEventArgs e)
        {
            if (ShipmentsGrid.SelectedItem is not ShipmentOverviewViewModel selected) return;

            var editWindow = new AddShipmentWindow(selected.Id);
            editWindow.ShowDialog();
            LoadShipments();
        }

        private async void DeleteShipment_Click(object sender, RoutedEventArgs e)
        {
            if (ShipmentsGrid.SelectedItem is not ShipmentOverviewViewModel selected) return;

            var result = MessageBox.Show("Are you sure you want to delete this shipment?", "Confirm", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                var trackedShipment = await _context.Shipments
                    .Include(s => s.ShipmentItems)
                    .FirstOrDefaultAsync(s => s.Id == selected.Id);

                if (trackedShipment != null)
                {
                    _context.ShipmentItems.RemoveRange(trackedShipment.ShipmentItems);
                    _context.Shipments.Remove(trackedShipment);
                    await _context.SaveChangesAsync();
                    LoadShipments();
                }
                else
                {
                    MessageBox.Show("Shipment not found or already deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                MessageBox.Show("This shipment was already modified or deleted by another operation.", "Concurrency Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the shipment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ShipmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShipmentsGrid.SelectedItem is ShipmentOverviewViewModel selectedShipment)
            {
                string destination = selectedShipment.RouteDisplay?.Split('➜').LastOrDefault()?.Trim();

                if (!string.IsNullOrWhiteSpace(destination))
                {
                    WeatherTextBlock.Text = $"Loading weather for {destination}...";
                    string weather = await _weatherService.GetCurrentWeatherAsync(destination);
                    WeatherTextBlock.Text = weather;
                }
                else
                {
                    WeatherTextBlock.Text = "No destination found.";
                }
            }
            else
            {
                WeatherTextBlock.Text = string.Empty;
            }
        }
    }

    public class ShipmentOverviewViewModel
    {
        public int Id { get; set; }
        public Truck Truck { get; set; }
        public string RouteDisplay { get; set; }
        public double TotalWeight { get; set; }
    }
}
