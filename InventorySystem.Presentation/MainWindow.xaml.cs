using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShipmentButton_Click(object sender, RoutedEventArgs e)
        {
            var overview = new ShipmentOverviewWindow();
            overview.ShowDialog();
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var inventory = new InventoryOverviewWindow(); // Make sure this name matches your class
            inventory.ShowDialog();
        }

        private void WarehouseButton_Click(object sender, RoutedEventArgs e)
        {
            var warehouse = new WarehouseWindow(); // ✅ Assumes WarehouseWindow.xaml exists
            warehouse.ShowDialog();
        }
    }
}
