============================================================
Inventory & Logistics Management System - Setup Guide (v1.0)
============================================================

Welcome! This document will walk you through setting up and running the Inventory & Logistics Management System on your local machine.

------------------------------------------------------------
1. Required Software
------------------------------------------------------------

✔ .NET SDK 8.0 (REQUIRED)
→ Download: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
→ Check installation:
    dotnet --version

✔ Visual Studio 2022 (RECOMMENDED)
→ Install the ".NET desktop development" workload
→ Also ensure Entity Framework Tools are installed

✔ Docker Desktop
→ Required for running SQL Server in a container
→ Download: https://www.docker.com/products/docker-desktop
→ Start Docker after installation (check system tray icon)

------------------------------------------------------------
2. Docker - Start SQL Server Container
------------------------------------------------------------

⚠ Required once per machine.

Open PowerShell or terminal and run:

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" ^
-p 1433:1433 --name sqlserver ^
-d mcr.microsoft.com/mssql/server:2022-latest

→ This will:
  • Pull and start SQL Server
  • Set login to: 
        User = sa
        Password = YourStrong!Passw0rd
  • Map to port 1433 on localhost

→ Wait ~15 seconds after starting to allow SQL Server to initialize.

------------------------------------------------------------
3. Open the Project
------------------------------------------------------------

✔ Open InventorySystem.sln using Visual Studio 2022

✔ Set the Presentation Layer as the Startup Project:
   - Right-click InventorySystem.Presentation → Set as Startup Project

------------------------------------------------------------
4. Run the Database Migration
------------------------------------------------------------

✔ This creates all tables from code.

→ Open:
   Tools → NuGet Package Manager → Package Manager Console

→ In the top-right dropdown, set "Default project" to:
   InventorySystem.Infrastructure

→ Then run:

Update-Database -StartupProject InventorySystem.Presentation

→ If successful, this applies the schema to your local SQL Server container.

------------------------------------------------------------
5. Login Credentials
------------------------------------------------------------

✔ Default seeded admin user:

   Username: admin
   Password: admin123
   Role: Admin

✔ You can register additional users using the "Register New User" button on the dashboard.

------------------------------------------------------------
6. Running the Application
------------------------------------------------------------

→ Press F5 or click "Start" in Visual Studio

→ Login with the admin user

→ From the Dashboard you can:
   • Manage Inventory
   • Manage Shipments
   • Assign and Edit Tasks
   • Register New Users

------------------------------------------------------------
7. Known Technical Notes
------------------------------------------------------------

• Connection string is currently hardcoded in each project’s AppDbContext:
   "Server=localhost,1433;Database=InventoryDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

→ In future versions, this should be moved to appsettings.json or .env for better security.

• Passwords are hashed with BCrypt.Net

• Projects auto-restore dependencies on build
