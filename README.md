# 📦 Inventory Management System

A modern logistics and inventory management application built with:

- **.NET 8 (ASP.NET Core Web API)** for backend services  
- **WPF (.NET)** for the desktop user interface  
- **SQL Server** for data persistence

---

## 🖥️ Features

- 🔐 Optional user authentication with JWT
- 📋 Inventory tracking with item weights and warehouse mapping
- 🚚 Shipment creation with truck capacity validation
- 🏭 Warehouse and route management
- 🗺️ Map view integration using WebView2 and third-party APIs
- ✨ Sleek and modern WPF UI

---

## 📁 Project Structure

InventorySystem/
├── Domain/ // Entity models
├── Application/ // DTOs and service interfaces
├── Infrastructure/ // DbContext and business logic implementations
├── Presentation/ // WPF desktop application


---

## 🚀 Getting Started

### 🛠 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- Visual Studio 2022 or Visual Studio Code

### 🔧 Setup Instructions

1. **Clone the repository**  
   ```bash
   git clone https://github.com/yourusername/InventorySystem.git
   cd InventorySystem
   
2. Configure the database

    Edit InventorySystem.WebApi/appsettings.json with your SQL Server connection string:

    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=InventorySystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }

3. Run Entity Framework migrations (if needed)

    dotnet ef database update --project InventorySystem.Infrastructure

4. Run the WPF application

    Open InventorySystem.sln in Visual Studio

    Set InventorySystem.Presentation as the startup project

    Press F5 to run



Sample API Call

POST /api/shipments?currentUserLocation=Sarajevo
Content-Type: application/json

{
  "destinationEntityId": 1,
  "destinationType": "Client",
  "truckId": 2,
  "items": [
    { "inventoryItemId": 3, "amount": 5 },
    { "inventoryItemId": 4, "amount": 10 }
  ]
}


License

MIT License.
Created as part of a university software engineering project.
