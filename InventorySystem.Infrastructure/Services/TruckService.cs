using InventorySystem.Domain.Models;

public class TruckService
{
    private List<Truck> _trucks = new List<Truck>();
    private List<TruckMaintenanceRecord> maintenanceRecords = new List<TruckMaintenanceRecord>();


    public void AddTruck(object truckObject)
    {
        if (truckObject is Truck truck)
        {
            _trucks.Add(truck); 
            Console.WriteLine($"Dodano vozilo s registracijom: {truck.LicensePlate}");
        }
        else
        {
            throw new InvalidCastException("Objekat nije tipa Truck");
        }
    }

    public void UpdateTruckStatus(object truckObject, string newStatus)
    {
        if (truckObject is Truck truck)
        {
            truck.Status = newStatus; 
            Console.WriteLine($"Status ažuriran na: {truck.Status}");
        }
        else
        {
            Console.WriteLine("Greška: Prosleđeni objekat nije tipa Truck");
        }
    }

    public Truck? FindTruckById(object truckObject)
    {
        if (truckObject is Truck truck)
        {
            return _trucks.FirstOrDefault(t => t.TruckId == truck.TruckId);
        }

        return null;
    }
    public IEnumerable<Truck> GetAllTrucks()

    {
        return _trucks;
    }

    public IEnumerable<TruckMaintenanceRecord> GetMaintenanceForTruck(int truckId)
    {
        return maintenanceRecords.Where(r => r.TruckId == truckId);
    }
    public void AddMaintenanceRecord(TruckMaintenanceRecord record)
    {
        maintenanceRecords.Add(record);
        var truck = _trucks.FirstOrDefault(t => t.TruckId == record.TruckId);
        if (truck != null)
        {
            truck.IsUnderMaintenance = true;
            truck.Status = "Unavailable";
        }
    }
    public void CompleteMaintenance(int recordId)
    {
        var record = maintenanceRecords.FirstOrDefault(r => r.Id == recordId);
        if (record != null)
        {
            record.Status = MaintenanceStatus.Completed;
            record.EndDate = DateTime.Now;

            var truck = _trucks.FirstOrDefault(t => t.TruckId == record.TruckId);
            if (truck != null)
            {
                // Is still active maintenance record for this truck
                bool stillInProgress = maintenanceRecords.Any(r => r.TruckId == truck.TruckId && r.Status != MaintenanceStatus.Completed);
                truck.IsUnderMaintenance = stillInProgress;
                truck.Status = stillInProgress ? "Unavailable" : "Available";
            }
        }
    }
    public Truck? FindAvailableTruck()
    {
        return _trucks.FirstOrDefault(t => !t.IsUnderMaintenance && t.Status == "Available");
    }
    public List<TruckMaintenanceRecord> GetAllMaintenanceRecords()
    {
        return maintenanceRecords;
    }


}
