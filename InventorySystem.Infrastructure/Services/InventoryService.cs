using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Domain.Models;
using InventorySystem.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Infrastructure.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public List<InventoryItem> GetAllItems()
        {
            return _context.InventoryItems.OrderBy(i => i.Name).ToList();
        }

        public void AddItem(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var item = _context.InventoryItems.Find(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}

