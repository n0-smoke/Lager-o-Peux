using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddTruckWindow : Window
    {
        private readonly AppDbContext _context;

        public AddTruckWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CapacityBox.Text, out int capacity))
            {
                MessageBox.Show("Please enter a valid numeric capacity.");
                return;
            }

            var truck = new Truck
            {
                Identifier = IdentifierBox.Text.Trim(),
                MaxCapacityKg = capacity
            };

            _context.Trucks.Add(truck);
            _context.SaveChanges();

            MessageBox.Show("Truck added successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
