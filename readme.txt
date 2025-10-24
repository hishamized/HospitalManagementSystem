
 Hospital Management System (HMS)

 Project Overview

This project is a Hospital Management System (HMS) built using ASP.NET Core with a Layered Onion Architecture and CQRS (Command Query Responsibility Segregation) pattern. It provides functionality to manage patients, appointments, doctors, departments, slots, and more. The system is designed to be scalable, maintainable, and easy to extend.



 Onion Architecture

The project follows the Onion Architecture principle, which ensures a clean separation of concerns and dependency direction:

1. Core Layer (Domain Layer)
   Contains entities, value objects, and domain logic. This layer has no dependencies on other layers and defines the core business rules.

2. Application Layer
   Contains DTOs, commands, queries, and service interfaces. Implements business logic using the domain entities and exposes interfaces to the infrastructure layer.

3. Infrastructure Layer
   Contains database context, repositories, email services, and other external dependencies. Implements interfaces defined in the application layer.

4. Presentation Layer (Web Layer)
   ASP.NET Core MVC controllers, views, and API endpoints. Handles HTTP requests, communicates with the application layer via CQRS commands and queries, and returns responses.



 CQRS Structure

The project implements CQRS (Command Query Responsibility Segregation):

 Commands: Handle operations that modify the system state (Create, Update, Delete). Each command has a dedicated Handler.
 Queries: Handle readonly operations to fetch data. Each query has a dedicated Handler.
 Mediator: The system uses MediatR to decouple command/query execution from controllers.



 Prerequisites

 .NET 6 or later
 SQL Server
 Visual Studio 2022 or VS Code
 Node.js (optional if frontend scripts are used)



 Setup and Running

 1. Using Migrations

1. Update the `appsettings.json` file with your SQL Server connection string.
   Example:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=HMS;Trusted_Connection=True;"
   }
   ```
2. Open Package Manager Console in Visual Studio and run:

   ```powershell
   UpdateDatabase
   ```

   This will create the database schema and seed initial data.

 2. Using Provided SQL File

1. Open SQL Server Management Studio (SSMS) or your preferred client.
2. Create a new database (e.g., `HMS`).
3. Execute the provided SQL file (`HMS.sql`) to create tables and seed initial data.



 Default Admin Credentials

To access the system as an admin:

 Username: `admin`
 Password: `qwertyuiop`



 Project Structure

```
HMS/
│
├─ Core/                   Domain Layer (Entities, Value Objects)
├─ Application/            Application Layer (DTOs, Commands, Queries, Interfaces)
├─ Infrastructure/         Infrastructure Layer (DbContext, Repositories, External Services)
└─ Web/                    Presentation Layer (Controllers, Views)
```



 Features

 Patient Management (CRUD)
 Patient Visit Management
 Medical History Management
 Allergy History Management
 Insurnce Management
 Doctor Management (CRUD)
 Department Management (CRUD)
 Appointment Scheduling and Rescheduling
 Slot Management
 Rolebased Authentication (Admin, User, etc.)
 CQRS implementation for clean separation of reads and writes
 Real Time Doctor availability




 How It Works

1. Controller receives HTTP requests from the client.
2. Commands/Queries are sent to MediatR for processing.
3. Handlers in the Application Layer execute business logic.
4. Repositories in the Infrastructure Layer interact with the database.
5. Responses are returned to the Controller, then to the client.

This ensures that read and write operations are separated, business logic is isolated, and dependencies flow inward following the Onion Architecture.



 Notes

 The system is built using ASP.NET Core MVC with layered architecture.
 The database can be regenerated either using EF Core Migrations or the provided SQL file.
 CQRS pattern ensures scalability and clean separation of concerns.


