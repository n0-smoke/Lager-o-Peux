using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using InventorySystem.Presentation.Session;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var password = PasswordBox.Password;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            using var context = new AppDbContext(optionsBuilder.Options);
            var userService = new UserService(context);

            var user = userService.Authenticate(username, password);

            if (user != null)
            {
                SessionManager.SetCurrentUser(user);

                MessageBox.Show($"Welcome, {user.Username} ({user.Role})!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

