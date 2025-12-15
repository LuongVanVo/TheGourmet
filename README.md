# 🍽️ TheGourmet

TheGourmet is a modern API system built on .NET 9 platform, applying Clean Architecture to ensure scalability, maintainability, and easy testing.

## 📋 Overview

This project is designed to provide a robust backend API, adhering to SOLID principles and Domain-Driven Design (DDD), suitable for large-scale applications.

## 🏗️ Architecture

The project is organized following **Clean Architecture** with 4 main layers:

```
TheGourmet/
├── src/
│   ├── TheGourmet.Domain/          # Entities, Value Objects, Domain Events
│   ├── TheGourmet.Application/     # Use Cases, DTOs, Interfaces
│   ├── TheGourmet.Infrastructure/  # Database, External Services
│   └── TheGourmet.Api/            # API Controllers, Middleware
├── docs/                          # Documentation
└── docker-compose.yml            # Container orchestration
```

### Dependency Flow
```
Api → Infrastructure → Application → Domain
```

## 🚀 Technology Stack

### Core Framework
- **.NET 9** - Main framework
- **ASP.NET Core Web API** - RESTful API

### Database & ORM
- **PostgreSQL** - Relational database management system
- **Entity Framework Core 9** - ORM
- **Npgsql** - PostgreSQL provider for EF Core

### Message Broker
- **RabbitMQ** - Message queue for asynchronous processing
- **MassTransit** - Framework for working with RabbitMQ

### Patterns & Libraries
- **MediatR** - CQRS pattern and Mediator
- **FluentValidation** - Validation logic
- **AutoMapper** - Object-to-object mapping

## 📦 Installation

### Requirements
- .NET 9 SDK
- Docker & Docker Compose (recommended)
- PostgreSQL 16+
- RabbitMQ

### Step 1: Clone repository
```bash
git clone https://github.com/LuongVanVo/TheGourmet.git
cd TheGourmet
```

### Step 2: Start Infrastructure Services
```bash
docker-compose up -d
```

### Step 3: Update Connection String
Edit `appsettings.Development.json` in the `TheGourmet.Api` project:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=thegourmet;Username=postgres;Password=yourpassword"
  }
}
```

### Step 4: Run Migration
```bash
cd src/TheGourmet.Api
dotnet ef database update --project ../TheGourmet.Infrastructure
```

### Step 5: Run the application
```bash
dotnet run --project src/TheGourmet.Api
```

The API will run at: `https://localhost:5001` (or configured port)

## 🔧 Development

### Restore dependencies
```bash
dotnet restore
```

### Build solution
```bash
dotnet build
```

### Run tests
```bash
dotnet test
```

### Create new Migration
```bash
dotnet ef migrations add MigrationName --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

## 📚 API Documentation

When running in Development mode, access Swagger UI at:
```
https://localhost:5001/swagger
```

### Health Check Endpoint
```bash
GET /api/health
```
Check the status of API and database connection.

## 🐳 Docker

### Run the entire stack with Docker Compose
```bash
docker-compose up -d
```

Services include:
- PostgreSQL (Port: 5432)
- RabbitMQ (Port: 5672, Management UI: 15672)

## 📖 Project Structure

### Domain Layer
Contains business logic and entities, independent of any other layers.
- Entities (BaseEntity)
- Value Objects
- Domain Events
- Domain Exceptions

### Application Layer
Contains use cases and business rules of the application.
- Commands & Queries (CQRS)
- DTOs
- Validators
- Mapping Profiles
- Interfaces

### Infrastructure Layer
Implements interfaces defined in the Application layer.
- DbContext & Migrations
- Repositories
- External Services
- Message Handlers

### API Layer
Presentation layer, handles HTTP requests.
- Controllers
- Middleware
- Filters
- API Configuration

## 🤝 Contributing

All contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is released under the MIT License.

## 👤 Author

**LuongVanVo**
- GitHub: [@LuongVanVo](https://github.com/LuongVanVo)

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- .NET Community

---
⭐ If you find this project useful, please give it a star!