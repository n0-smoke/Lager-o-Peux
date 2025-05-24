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
            var shipmentWindow = new AddShipmentWindow();
            shipmentWindow.Show();
        }
    }
}
