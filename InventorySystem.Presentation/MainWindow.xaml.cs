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

            inventoryWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            inventoryWindow.Left = 552;
            inventoryWindow.Top = 196;
            inventoryWindow.Width = 944;
            inventoryWindow.Height = 596;

            inventoryWindow.ShowDialog();
        }


        private void OpenShipment_Click(object sender, RoutedEventArgs e)
        {
            var shipmentWindow = new ShipmentWindow();

            shipmentWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            shipmentWindow.Left = 552;
            shipmentWindow.Top = 196;
            shipmentWindow.Width = 944;
            shipmentWindow.Height = 596;

            shipmentWindow.ShowDialog();
        }

        private void OpenTasks_Click(object sender, RoutedEventArgs e)
        {
            var taskWindow = new TaskWindow();

            taskWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            taskWindow.Left = 552;
            taskWindow.Top = 196;
            taskWindow.Width = 944;
            taskWindow.Height = 596;
            taskWindow.ShowDialog();
        }

        private void OpenAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Analytics Dashboard window goes here.");
        }
        private void RegisterUser_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterUserWindow();

            registerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            registerWindow.Left = 552;
            registerWindow.Top = 196;
            registerWindow.Width = 944;
            registerWindow.Height = 596;
            registerWindow.ShowDialog();
        }

    }
}
