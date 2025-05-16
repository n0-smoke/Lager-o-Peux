using System.Linq;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class TruckWindow : Window
    {
        private readonly AppDbContext _context;

        public TruckWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);

            LoadData();
        }

        private void LoadData()
        {
            TruckGrid.ItemsSource = _context.Trucks.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTruckWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = TruckGrid.SelectedItem as Truck;
            if (selected == null)
            {
                MessageBox.Show("Please select a truck to edit.");
                return;
            }

            var editWindow = new EditTruckWindow(selected);
            if (editWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = TruckGrid.SelectedItem as Truck;
            if (selected == null)
            {
                MessageBox.Show("Please select a truck to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete truck '{selected.Identifier}'?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                _context.Trucks.Remove(selected);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Truck deleted.");
            }
        }

    }
}
