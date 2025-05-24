using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Domain.Models;

namespace InventorySystem.Presentation.Session
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; set; }
    }
}

