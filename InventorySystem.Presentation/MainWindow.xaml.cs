using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InventorySystem.Presentation.Session;

namespace InventorySystem.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var user = SessionManager.CurrentUser;

            if (user != null)
            {
                WelcomeText.Text = $"Welcome, {user.Username} ({user.Role})";
            }
        }

        private void OpenInventory_Click(object sender, RoutedEventArgs e)
        {
            var inventoryWindow = new InventoryWindow();
            inventoryWindow.Show();
        }

        private void OpenShipment_Click(object sender, RoutedEventArgs e)
        {
            var shipmentWindow = new ShipmentWindow();
            shipmentWindow.Show();
        }

        private void OpenTasks_Click(object sender, RoutedEventArgs e)
        {
            var taskWindow = new TaskWindow();
            taskWindow.ShowDialog();
        }

        private void OpenAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Analytics Dashboard window goes here.");
        }
        private void RegisterUser_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterUserWindow();
            registerWindow.ShowDialog();
        }
        private void OpenTruck_Click(object sender, RoutedEventArgs e)
        {
            var truckWindow = new TruckWindow();
            truckWindow.ShowDialog();
        }

    }
}
