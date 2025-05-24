using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AssignItemsWindow : Window
    {
        private readonly AppDbContext _context;
        private List<ShipmentInventoryItem> _items;

        public List<ShipmentInventoryItem> SelectedItems { get; private set; } = new();

        public AssignItemsWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);

            LoadInventoryItems();

            ItemGrid.CellEditEnding += ItemGrid_CellEditEnding;
        }

        private void LoadInventoryItems()
        {
            var items = _context.InventoryItems.ToList();

            _items = items.Select(i => new ShipmentInventoryItem
            {
                InventoryItemId = i.Id,
                InventoryItem = i,
                Quantity = 0
            }).ToList();

            ItemGrid.ItemsSource = _items;
            UpdateSummary();
        }

        private void ItemGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(() => UpdateSummary(), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void UpdateSummary()
        {
            var selected = _items.Where(i => i.Quantity > 0).ToList();

            double totalWeight = selected.Sum(i =>
                i.InventoryItem.WeightPerUnit * i.Quantity);

            ItemCountLabel.Text = selected.Count.ToString();
            TotalWeightLabel.Text = $"{totalWeight:0.##} kg";

            // Refresh UI
            if (ItemGrid.CommitEdit(DataGridEditingUnit.Cell, true) &&
                ItemGrid.CommitEdit(DataGridEditingUnit.Row, true))
            {
                ItemGrid.Items.Refresh();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            SelectedItems = _items.Where(i => i.Quantity > 0).ToList();

            if (!SelectedItems.Any())
            {
                MessageBox.Show("Please assign at least one item.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
