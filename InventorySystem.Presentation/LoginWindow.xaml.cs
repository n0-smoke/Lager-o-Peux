using System.Linq;
using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
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
            var username = UsernameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorTextBlock.Text = "Please enter both username and password.";
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    ErrorTextBlock.Text = "Invalid username or password.";
                    return;
                }

                // Successful login
                SessionManager.CurrentUser = user;

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
