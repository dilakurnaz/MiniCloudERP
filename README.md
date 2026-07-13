# MiniCloudERP

A simplified Cloud ERP system developed with **ASP.NET Core and Blazor Server**.

MiniCloudERP is a web-based ERP application designed to manage basic business operations such as customer management, product management, user authentication, and database operations.

The project demonstrates a modern .NET application architecture using layered design principles, Entity Framework Core, SQL Server, and ASP.NET Core Identity.



## 🚀 Features

- User authentication and authorization
- Customer management
- Product management
- SQL Server database integration
- Entity Framework Core database operations
- ASP.NET Core Identity integration
- Blazor Server interactive user interface
- Layered project architecture



## 🛠 Technologies

| Technology | Description |
|---|---|
| .NET 8 | Application framework |
| ASP.NET Core | Backend framework |
| Blazor Server | Frontend web framework |
| Entity Framework Core | ORM and database management |
| SQL Server | Database system |
| ASP.NET Core Identity | Authentication and authorization |
| C# | Programming language |
| Git | Version control |



## 📂 Project Structure

```
MiniCloudERP
│
├── src
│   │
│   ├── MiniCloudERP.Web
│   │   └── Blazor Server web application
│   │
│   ├── MiniCloudERP.DataAccess
│   │   └── Database context and data operations
│   │
│   └── MiniCloudERP.Domain
│       └── Entity models
│
├── MiniCloudERP.sln
├── global.json
└── README.md
```

The project follows a layered architecture:

### Domain Layer
Contains the core entity models and business objects.

### Data Access Layer
Handles database communication using Entity Framework Core.

### Web Layer
Provides the user interface and application logic using Blazor Server.



# ⚙️ Requirements

Before running the project, make sure you have:

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 / Rider / VS Code
- Entity Framework Core Tools

Check your installed .NET version:

```bash
dotnet --version
```



# 📥 Installation

Clone the repository:

```bash
git clone https://github.com/dilakurnaz/MiniCloudERP.git
```

Navigate to the project folder:

```bash
cd MiniCloudERP
```

Restore project dependencies:

```bash
dotnet restore
```



# 🗄 Database Configuration

The application uses **SQL Server** as the database provider.

Before running the application, update the connection string according to your local SQL Server configuration.

Connection string location:

```
src/MiniCloudERP.Web/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MiniCloudERP;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```



# 🏗 Database Migration

Install Entity Framework Core tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

Apply database migrations:

```bash
dotnet ef database update --project src/MiniCloudERP.DataAccess --startup-project src/MiniCloudERP.Web
```



# ▶️ Running the Application

Run the application:

```bash
dotnet run --project src/MiniCloudERP.Web
```

After the application starts successfully, open the URL shown in the terminal:

Example:

```
https://localhost:xxxx
```

or

```
http://localhost:xxxx
```



# 🔐 Authentication

The project uses **ASP.NET Core Identity** for user authentication and authorization.

Supported features:

- User registration
- User login
- User authentication management
- Authorization support



# 🧩 Development Notes

- The application is developed using a layered architecture approach.
- Entity Framework Core is used for database operations.
- SQL Server is used as the primary database provider.
- Configuration settings should be updated according to the development environment.


# 📌 Future Improvements

Possible future enhancements:

- Advanced ERP modules
- Role-based permission management
- Reporting dashboard
- Inventory tracking system
- Cloud deployment
- API integrations
