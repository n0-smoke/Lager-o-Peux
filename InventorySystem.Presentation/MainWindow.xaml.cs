using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;

namespace InventorySystem.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async();

            // 🔧 Local development frontend (React/Vite dev server)
            webView.Source = new Uri("http://localhost:5173");

            // Listen to messages from the frontend
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();

            switch (message)
            {
                case "open-shipments":
                    new ShipmentOverviewWindow().Show();
                    break;

                case "open-inventory":
                    new InventoryOverviewWindow().Show();
                    break;

                case "open-warehouses":
                    new WarehouseWindow().Show();
                    break;

                case "open-map":
                    new WarehouseMapWindow().Show();
                    break;

                default:
                    MessageBox.Show($"Unknown message: {message}");
                    break;
            }
        }
    }
}
