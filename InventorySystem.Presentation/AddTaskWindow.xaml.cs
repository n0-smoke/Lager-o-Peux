using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class AddTaskWindow : Window
    {
        private readonly TaskService _taskService;
        private readonly AppDbContext _context;

        public AddTaskWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _taskService = new TaskService(_context);

            LoadUsers();
            DueDatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadUsers()
        {
            var users = _context.Users.ToList();
            UserBox.ItemsSource = users;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (UserBox.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Please select a user to assign the task to.");
                return;
            }

            var task = new TaskAssignment
            {
                Title = TitleBox.Text,
                Description = DescriptionBox.Text,
                Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending",
                DueDate = DueDatePicker.SelectedDate ?? DateTime.Now,
                AssignedToUserId = selectedUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            _taskService.AddTask(task);

            MessageBox.Show("Task successfully assigned.");
            this.DialogResult = true;
            this.Close();
        }
    }
}

