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
    public partial class EditTaskWindow : Window
    {
        private readonly TaskService _taskService;
        private readonly AppDbContext _context;
        private readonly TaskAssignment _task;

        public EditTaskWindow(TaskAssignment task)
        {
            InitializeComponent();
            _task = task;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _taskService = new TaskService(_context);

            LoadUsers();

            // Fill form with existing data
            TitleBox.Text = _task.Title;
            DescriptionBox.Text = _task.Description;
            DueDatePicker.SelectedDate = _task.DueDate;

            foreach (ComboBoxItem item in StatusBox.Items)
            {
                if (item.Content?.ToString() == _task.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            UserBox.SelectedValue = _task.AssignedToUserId;
        }

        private void LoadUsers()
        {
            var users = _context.Users.ToList();
            UserBox.ItemsSource = users;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (UserBox.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Please select a user.");
                return;
            }

            _task.Title = TitleBox.Text;
            _task.Description = DescriptionBox.Text;
            _task.Status = ((ComboBoxItem)StatusBox.SelectedItem)?.Content?.ToString() ?? "Pending";
            _task.DueDate = DueDatePicker.SelectedDate ?? DateTime.Now;
            _task.AssignedToUserId = selectedUser.Id;

            _taskService.UpdateTask(_task);

            MessageBox.Show("Task updated successfully.");
            this.DialogResult = true;
            this.Close();
        }
    }
}
