# 🚗 Car Management System

A modern web-based **Car Management System** built with **ASP.NET Core MVC** that enables users to efficiently manage their vehicles and service orders in a secure and structured environment.

---

## 📋 Table of Contents
- 📖 About the Project
- ✨ Key Features
- 🛠 Technologies Used
- 🏗 Project Architecture
- 📋 Prerequisites
- 🚀 Installation & Setup
- 💾 Database Setup
- 🎯 Usage
- 🔑 Default Admin Account
- 📁 Project Structure
- 🔒 Security Features
- 📝 License

---

## 📖 About the Project

The **Car Management System** is a full-stack web application designed to help users organize and track their vehicles along with associated service and maintenance operations.

Each user has a **secure, isolated workspace**, allowing them to:
- Register and manage multiple vehicles
- Create and monitor service orders
- Track maintenance history over time

The application focuses on **simplicity, scalability, and clean architecture practices**.

---

## ✨ Key Features

### 🚘 Car Management
- Add vehicles with detailed information (brand, model, year, VIN, owner details)
- View all registered cars in a structured list
- Edit and update vehicle information
- Delete cars (with automatic removal of related service orders)
- View full car details including service history

### 🧰 Service Order Management
- Create service orders linked to specific cars
- Add descriptions and estimated pricing
- Track service progress:
  - `Pending`
  - `In Progress`
  - `Completed`
- Edit and update service orders
- Delete service records
- Easy car selection with clear model identification

### 👤 User Management
- Secure user registration and login via ASP.NET Core Identity
- Password-based authentication (minimum 6 characters)
- User-specific data isolation (each user sees only their own data)
- Simplified setup (email confirmation disabled for development)

---

## 🛠 Technologies Used

### 🔧 Backend
- **.NET 10**
- **ASP.NET Core MVC**
- **ASP.NET Core Identity**
- **Entity Framework Core (EF Core)**
- **SQL Server** (default)
- Optional: **SQLite**

### 🎨 Frontend
- Razor Views
- Bootstrap
- HTML5 & CSS3

### 🧱 Architecture & Patterns
- Layered Architecture
- Repository Pattern
- MVVM (via ViewModels)
- Dependency Injection

---

## 🏗 Project Architecture

The solution follows a clean, modular architecture:


Car Management/

├── Presentation Layer 

│ └── Car Manigment/ # ASP.NET Core MVC Web App

│ ├── Controllers/ # Handles HTTP requests

│ ├── Views/ # Razor UI

│ ├── wwwroot/ # Static files (CSS, JS, images)

│ └── Program.cs # Application entry point

│

├── Business Logic Layer

│ └── CarManigment.Services.Core/ # Core application logic & services

│

├── Data Access Layer

│ └── CarManigment.Data/ # Database access

│ └── ApplicationDbContext.cs # EF Core DbContext

│

├── Domain Layer

│ └── CarManigment.Data.Models/ # Core entities

│ ├── Car.cs # Car entity

│ ├── ServiceOrder.cs # Service order entity

│ └── ServiceStatus.cs # Service status enum

│

├── ViewModel Layer

│ └── CarManigment.ViewModels/ # Data for UI

│ ├── Cars/ # Car ViewModels

│ └── ServiceOrders/ # Service order ViewModels

│

└── Common Layer

└── CarManigment.Common/ # Shared utilities

└── ValidationConstants.cs # Validation rules

---

## 📋 Prerequisites

Make sure you have:

- .NET 10 SDK
- SQL Server (Express or higher)
- Visual Studio 2022 or VS Code
- Git

---

## 🚀 Installation & Setup

### 1. Clone the repository

```bash
git clone https://github.com/Babati14/Car-Manigment.git
cd Car-Manigment
2. Configure the database

Edit:

Car Manigment/appsettings.json

Example:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CarManagementDb;Trusted_Connection=True;"
}
3. Restore dependencies
dotnet restore
💾 Database Setup
Apply migrations
cd "Car Manigment"
dotnet ef database update
Create new migration (if needed)
dotnet ef migrations add YourMigrationName --project ../CarManigment.Data
dotnet ef database update
🎯 Usage
Run the application
cd "Car Manigment"
dotnet run

Open in browser:

https://localhost:5001
First Steps
1. Register
Create an account using email + password
2. Add a Car
Navigate to Cars → Create
Enter:
Brand
Model
Year
VIN (17 chars)
Owner info
3. Create Service Order
Go to Service Orders → Create
Select car
Add description + price
🔑 Default Admin Account

On application startup, the system automatically seeds an Admin user and role (if they do not already exist).

You can log in using:

Email: admin@carmanagement.com
Password: Admin123

⚠️ Important: This account is intended for development/testing purposes only.
Make sure to change or remove these credentials in a production environment.

📁 Project Structure
Controllers
CarsController – Car CRUD
ServiceOrdersController – Service logic
HomeController – General routes
Models
Car – Vehicle entity
ServiceOrder – Service tracking
ServiceStatus – Enum (Pending, In Progress, Completed)
ViewModels
Separate models for Create/Edit/List/Details
Input validation included
Data Layer
ApplicationDbContext
Handles EF Core operations & migrations
Common
Validation constants
Shared utilities
🔒 Security Features
ASP.NET Core Identity authentication
Authorization (login required)
User data isolation
Anti-forgery (CSRF protection)
SQL injection protection via EF Core
HTTPS enforced by default

This project is open-source. See the repository for license details.

👤 Author

Babati14
GitHub: https://github.com/Babati14
  
---

**Happy Car Managing! 🚗**
