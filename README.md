# Car Management System

A comprehensive web-based Car Management System built with ASP.NET Core MVC that allows users to manage their vehicles and service orders efficiently.

## 📋 Table of Contents

- [About the Project]
- [Key Features]
- [Technologies Used]
- [Project Architecture]
- [Prerequisites]
- [Installation & Setup]
- [Database Setup]
- [Usage]
- [Project Structure]
- [Contributing]

## 📖 About the Project

Car Management System is a web application designed to help users track and manage their vehicles along with their service orders. Users can register their cars, create service orders, and monitor the status of maintenance work. Each user has their own secure space to manage their vehicles, ensuring data privacy and personalized experience.

## ✨ Key Features

### Car Management
- **Add New Cars**: Register vehicles with detailed information including brand, model, year, VIN number, and owner details
- **View Car List**: Browse all your registered vehicles in an organized manner
- **Update Car Information**: Edit vehicle details as needed
- **Delete Cars**: Remove vehicles from your account (with cascade deletion of associated service orders)
- **View Car Details**: See comprehensive information about each vehicle including service history

### Service Order Management
- **Create Service Orders**: Generate new service orders for your vehicles with descriptions and estimated prices
- **Track Service Status**: Monitor service orders through different statuses (Pending, In Progress, Completed)
- **View Service History**: Access all service orders for your vehicles
- **Update Service Orders**: Modify service descriptions, prices, status, and associated vehicles
- **Delete Service Orders**: Remove completed or cancelled service orders
- **Car Selection with Model Display**: Easily identify cars when creating service orders with ID and model information

### User Management
- **User Registration**: Create new accounts with email verification disabled for easy testing
- **User Authentication**: Secure login system powered by ASP.NET Core Identity
- **Password Requirements**: Flexible password rules (minimum 6 characters)
- **User Isolation**: Each user can only access and manage their own cars and service orders

## 🛠 Technologies Used

### Backend
- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core MVC** - Web framework for building the application
- **ASP.NET Core Identity** - Authentication and authorization
- **Entity Framework Core 10.0.3** - ORM for database operations
- **SQL Server** - Primary database (with SQLite support available)

### Frontend
- **Razor Views** - Server-side rendering
- **Bootstrap** - Responsive UI framework
- **HTML5 & CSS3** - Modern web standards

### Architecture Patterns
- **Repository Pattern** - Data access abstraction
- **MVVM Pattern** - Separation of concerns with ViewModels
- **Dependency Injection** - Built-in ASP.NET Core DI container
- **Layered Architecture** - Clear separation of concerns

## 🏗 Project Architecture

The solution follows a clean, layered architecture pattern:

```
Car Management/
├── Car Manigment/                    # Main web application (MVC)
│   ├── Controllers/                  # MVC Controllers
│   ├── Views/                        # Razor Views
│   ├── wwwroot/                      # Static files
│   └── Program.cs                    # Application entry point
├── CarManigment.Data/                # Data access layer
│   └── ApplicationDbContext.cs       # EF Core DbContext
├── CarManigment.Data.Models/         # Domain models
│   ├── Car.cs                        # Car entity
│   ├── ServiceOrder.cs               # Service order entity
│   └── ServiceStatus.cs              # Service status enum
├── CarManigment.ViewModels/          # View models for MVC
│   ├── Cars/                         # Car-related ViewModels
│   └── ServiceOrders/                # Service order ViewModels
├── CarManigment.Services.Core/       # Business logic layer
└── CarManigment.Common/              # Shared constants and utilities
    └── ValidationConstants.cs        # Validation rules
```

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition or higher)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or Visual Studio Code
- [Git](https://git-scm.com/downloads) (for cloning the repository)

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Babati14/Car-Manigment.git
cd Car-Manigment
```

### 2. Configure Connection String

Update the connection string in `appsettings.json` located in the `Car Manigment` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CarManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Note**: Adjust the connection string based on your SQL Server configuration:
- For SQL Server Express: `Server=.\\SQLEXPRESS;Database=CarManagementDb;Trusted_Connection=True;`
- For SQL Server with authentication: `Server=YOUR_SERVER;Database=CarManagementDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;`

### 3. Restore NuGet Packages

```bash
dotnet restore
```

## 💾 Database Setup

### Apply Migrations

Navigate to the main project directory and run:

```bash
cd "Car Manigment"
dotnet ef database update
```

This will create the database and apply all migrations, setting up the following tables:
- `AspNetUsers` - User accounts
- `Cars` - Vehicle information
- `ServiceOrders` - Service order records
- Additional Identity tables for authentication

### Create New Migration (Optional)

If you make changes to the models:

```bash
dotnet ef migrations add YourMigrationName --project ../CarManigment.Data
dotnet ef database update
```

## 🎯 Usage

### Running the Application

1. **Using Visual Studio**:
   - Open `Car Manigment.sln`
   - Set `Car Manigment` as the startup project
   - Press `F5` or click the "Run" button

2. **Using Command Line**:
   ```bash
   cd "Car Manigment"
   dotnet run
   ```

3. **Access the application**:
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
   - The exact port will be displayed in the console output

### First-Time Setup

1. **Register a New Account**:
   - Click on "Register" in the top navigation
   - Fill in your email and password (minimum 6 characters)
   - Click "Register" to create your account

2. **Add Your First Car**:
   - Navigate to "Cars" in the menu
   - Click "Create New"
   - Fill in the car details:
     - Brand (e.g., Toyota, Honda)
     - Model (e.g., Camry, Accord)
     - Year (1980-2100)
     - VIN Number (17 characters)
     - Owner Name
     - Owner Phone
   - Click "Create"

3. **Create a Service Order**:
   - Navigate to "Service Orders"
   - Click "Create New"
   - Select a car from the dropdown (shows ID - Brand Model)
   - Enter service description
   - Set estimated price
   - Click "Create"

## 📁 Project Structure

### Main Application (Car Manigment)
- **Controllers**: Handle HTTP requests and responses
  - `CarsController.cs` - Car CRUD operations
  - `ServiceOrdersController.cs` - Service order management
  - `HomeController.cs` - Landing page and general routes

- **Views**: Razor templates for rendering HTML
  - `/Cars` - Car management views
  - `/ServiceOrders` - Service order views
  - `/Shared` - Shared layout and partial views

### Data Layer (CarManigment.Data)
- **ApplicationDbContext.cs**: EF Core context with entity configurations
- Handles database connections and migrations

### Models (CarManigment.Data.Models)
- **Car.cs**: Vehicle entity with properties and relationships
- **ServiceOrder.cs**: Service order entity with status tracking
- **ServiceStatus.cs**: Enum for service order states

### ViewModels (CarManigment.ViewModels)
- Data transfer objects for views
- Separate ViewModels for Create, Edit, List, and Details operations
- Input validation attributes

### Common (CarManigment.Common)
- **ValidationConstants.cs**: Centralized validation rules and error messages
- Ensures consistent validation across the application

## 🔒 Security Features

- **Authentication**: ASP.NET Core Identity with secure password hashing
- **Authorization**: Only authenticated users can access car and service features
- **User Isolation**: Users can only view and modify their own data
- **HTTPS**: Secure communication (enabled by default)
- **Anti-Forgery Tokens**: Protection against CSRF attacks
- **SQL Injection Prevention**: Entity Framework parameterized queries

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute to this project:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is available as open source. Please check the repository for license details.

## 👤 Author

**Babati14**
- GitHub: [@Babati14](https://github.com/Babati14)
  
---

**Happy Car Managing! 🚗**
