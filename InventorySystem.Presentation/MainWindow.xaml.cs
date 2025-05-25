using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenInventory_Click(object sender, RoutedEventArgs e)
        {
            new InventoryOverviewWindow().ShowDialog();
        }

        private void OpenShipments_Click(object sender, RoutedEventArgs e)
        {
            new ShipmentOverviewWindow().ShowDialog();
        }

        private void OpenWarehouses_Click(object sender, RoutedEventArgs e)
        {
            new WarehouseWindow().ShowDialog();
        }

        private void OpenTrucks_Click(object sender, RoutedEventArgs e)
        {
            new TruckWindow().ShowDialog();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            new WarehouseMapWindow().ShowDialog();
        }
    }
}
