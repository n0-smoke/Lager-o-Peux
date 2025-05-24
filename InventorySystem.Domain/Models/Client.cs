using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace InventorySystem.Domain.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
}
