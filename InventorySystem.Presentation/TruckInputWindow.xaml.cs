using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class TruckInputWindow : Window
    {
        public string LicensePlate => LicensePlateTextBox.Text;
        public string Model => ModelTextBox.Text;
        public string Name => NameTextBox.Text;

        public TruckInputWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LicensePlate) ||
                string.IsNullOrWhiteSpace(Model) ||
                string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
