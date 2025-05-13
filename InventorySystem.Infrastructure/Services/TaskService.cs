using System.Collections.Generic;
using System.Linq;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public List<TaskAssignment> GetAllTasks()
        {
            return _context.TaskAssignments
                           .Include(t => t.AssignedToUser)
                           .OrderByDescending(t => t.CreatedAt)
                           .ToList();
        }

        public void AddTask(TaskAssignment task)
        {
            _context.TaskAssignments.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(TaskAssignment task)
        {
            _context.TaskAssignments.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = _context.TaskAssignments.Find(id);
            if (task != null)
            {
                _context.TaskAssignments.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}

