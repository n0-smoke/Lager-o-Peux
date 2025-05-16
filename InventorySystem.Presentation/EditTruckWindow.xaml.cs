using System;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class EditTruckWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Truck _truck;

        public EditTruckWindow(Truck truck)
        {
            InitializeComponent();
            _truck = truck;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);

            // Populate fields
            IdentifierBox.Text = _truck.Identifier;
            CapacityBox.Text = _truck.MaxCapacityKg.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CapacityBox.Text, out int capacity))
            {
                MessageBox.Show("Please enter a valid numeric capacity.");
                return;
            }

            _truck.Identifier = IdentifierBox.Text.Trim();
            _truck.MaxCapacityKg = capacity;

            _context.Trucks.Update(_truck);
            _context.SaveChanges();

            MessageBox.Show("Truck updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
