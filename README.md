# 🍽️ TheGourmet

TheGourmet là một hệ thống API hiện đại được xây dựng trên nền tảng .NET 9, áp dụng kiến trúc Clean Architecture để đảm bảo tính mở rộng, bảo trì và kiểm thử dễ dàng.

## 📋 Tổng quan

Dự án này được thiết kế để cung cấp một API backend mạnh mẽ, tuân thủ các nguyên tắc SOLID và Domain-Driven Design (DDD), phù hợp cho các ứng dụng quy mô lớn.

## 🏗️ Kiến trúc

Dự án được tổ chức theo **Clean Architecture** với 4 layers chính:

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

## 🚀 Công nghệ sử dụng

### Core Framework
- **.NET 9** - Framework chính
- **ASP.NET Core Web API** - RESTful API

### Database & ORM
- **PostgreSQL** - Hệ quản trị cơ sở dữ liệu quan hệ
- **Entity Framework Core 9** - ORM
- **Npgsql** - PostgreSQL provider cho EF Core

### Message Broker
- **RabbitMQ** - Message queue để xử lý bất đồng bộ
- **MassTransit** - Framework để làm việc với RabbitMQ

### Patterns & Libraries
- **MediatR** - CQRS pattern và Mediator
- **FluentValidation** - Validation logic
- **AutoMapper** - Object-to-object mapping

## 📦 Cài đặt

### Yêu cầu
- .NET 9 SDK
- Docker & Docker Compose (khuyến nghị)
- PostgreSQL 16+
- RabbitMQ

### Bước 1: Clone repository
```bash
git clone https://github.com/LuongVanVo/TheGourmet.git
cd TheGourmet
```

### Bước 2: Khởi động Infrastructure Services
```bash
docker-compose up -d
```

### Bước 3: Cập nhật Connection String
Chỉnh sửa `appsettings.Development.json` trong project `TheGourmet.Api`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=thegourmet;Username=postgres;Password=yourpassword"
  }
}
```

### Bước 4: Chạy Migration
```bash
cd src/TheGourmet.Api
dotnet ef database update --project ../TheGourmet.Infrastructure
```

### Bước 5: Chạy ứng dụng
```bash
dotnet run --project src/TheGourmet.Api
```

API sẽ chạy tại: `https://localhost:5001` (hoặc port được cấu hình)

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

### Tạo Migration mới
```bash
dotnet ef migrations add MigrationName --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

## 📚 API Documentation

Khi chạy ở chế độ Development, truy cập Swagger UI tại: 
```
https://localhost:5001/swagger
```

### Health Check Endpoint
```bash
GET /api/health
```
Kiểm tra trạng thái của API và kết nối database.

## 🐳 Docker

### Chạy toàn bộ stack với Docker Compose
```bash
docker-compose up -d
```

Services bao gồm:
- PostgreSQL (Port: 5432)
- RabbitMQ (Port: 5672, Management UI: 15672)

## 📖 Project Structure

### Domain Layer
Chứa business logic và entities, không phụ thuộc vào layer nào khác.
- Entities (BaseEntity)
- Value Objects
- Domain Events
- Domain Exceptions

### Application Layer
Chứa use cases và business rules của ứng dụng.
- Commands & Queries (CQRS)
- DTOs
- Validators
- Mapping Profiles
- Interfaces

### Infrastructure Layer
Triển khai các interfaces được định nghĩa trong Application layer.
- DbContext & Migrations
- Repositories
- External Services
- Message Handlers

### API Layer
Presentation layer, xử lý HTTP requests.
- Controllers
- Middleware
- Filters
- API Configuration

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Vui lòng:
1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📝 License

Dự án này được phát hành dưới MIT License.

## 👤 Tác giả

**LuongVanVo**
- GitHub: [@LuongVanVo](https://github.com/LuongVanVo)

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- .NET Community

---
⭐ Nếu bạn thấy project hữu ích, hãy cho một star nhé!