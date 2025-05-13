using System.Windows;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using InventorySystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Presentation
{
    public partial class TaskWindow : Window
    {
        private readonly TaskService _taskService;
        private readonly AppDbContext _context;

        public TaskWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True");

            _context = new AppDbContext(optionsBuilder.Options);
            _taskService = new TaskService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var tasks = _taskService.GetAllTasks();
            TaskGrid.ItemsSource = tasks;
        }
         
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTaskWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TaskGrid.SelectedItem as TaskAssignment;

            if (selectedTask == null)
            {
                MessageBox.Show("Please select a task to edit.");
                return;
            }

            var editWindow = new EditTaskWindow(selectedTask);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TaskGrid.SelectedItem as TaskAssignment;

            if (selectedTask == null)
            {
                MessageBox.Show("Please select a task to delete.");
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete the task: '{selectedTask.Title}'?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                _taskService.DeleteTask(selectedTask.Id);
                LoadData();
                MessageBox.Show("Task deleted.");
            }
        }

    }
}
