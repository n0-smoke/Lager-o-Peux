using System.Windows;
using System.Windows.Controls;
using BCrypt.Net;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class RegisterUserWindow : Window
    {
        private readonly AppDbContext _context;

        public RegisterUserWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password.Trim();
            var role = ((ComboBoxItem)RoleBox.SelectedItem)?.Content?.ToString() ?? "Employee";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and password are required.");
                return;
            }

            // Check if user exists
            if (_context.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            MessageBox.Show("User registered successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
