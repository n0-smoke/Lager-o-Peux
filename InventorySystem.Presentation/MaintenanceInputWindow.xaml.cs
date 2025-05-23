using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class MaintenanceInputWindow : Window
    {
        public string Description => DescriptionTextBox.Text;

        public MaintenanceInputWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
