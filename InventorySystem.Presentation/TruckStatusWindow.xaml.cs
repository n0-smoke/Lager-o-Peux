using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class TruckStatusWindow : Window
    {
        public string NewStatus => StatusComboBox.SelectedItem?.ToString() ?? "";

        public TruckStatusWindow(string currentStatus)
        {
            InitializeComponent();
            StatusComboBox.Items.Add("Available");
            StatusComboBox.Items.Add("Unavailable");
            StatusComboBox.SelectedItem = currentStatus;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NewStatus))
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
