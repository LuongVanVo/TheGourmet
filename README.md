# 🍽️ TheGourmet

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-336791?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?style=flat&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

TheGourmet is a modern API system built on .NET 9 platform, applying Clean Architecture to ensure scalability, maintainability, and easy testing.

## 📋 Overview

This project is designed to provide a robust backend API, adhering to SOLID principles and Domain-Driven Design (DDD), suitable for large-scale applications.

## ✨ Features

### 🔐 Authentication & Authorization
- User registration với email confirmation
- Login/Logout với JWT Bearer token
- Refresh token mechanism với cookie-based storage
- Forgot password & reset password functionality
- Role-based authorization (Admin, Customer)
- User profile management

### 🛍️ Product Management
- CRUD operations cho products (Admin only)
- Product pagination và filtering
- Product status toggle (active/inactive)
- Image upload integration với Cloudinary
- Stock quantity tracking
- Original price và sale price support

### 📂 Category Management
- CRUD operations cho categories (Admin only)
- Hierarchical category structure (Parent-Child relationships)
- Category tree visualization
- Soft delete support

### 🛒 Shopping Cart
- Add/remove items từ cart
- Update product quantity trong cart
- Clear entire cart
- User-specific cart management
- Real-time cart calculation

### 📁 File Management
- Avatar upload
- Product image upload
- Cloudinary integration cho cloud storage
- Role-based upload permissions

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

## 🎯 Design Patterns & Principles

### Patterns Implemented
- ✅ **Clean Architecture** - Tách biệt concerns và dependencies
- ✅ **CQRS (Command Query Responsibility Segregation)** - Tách read và write operations
- ✅ **Mediator Pattern** - Sử dụng MediatR
- ✅ **Repository Pattern** - Abstraction cho data access
- ✅ **Unit of Work** - Transaction management
- ✅ **Dependency Injection** - IoC container
- ✅ **Pipeline Behavior** - Cross-cutting concerns (validation)
- ✅ **Background Task Queue** - Async processing

### Principles
- ✅ **SOLID Principles**
- ✅ **Domain-Driven Design (DDD)**
- ✅ **Separation of Concerns**
- ✅ **Single Responsibility**

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

### External Services & Integrations
- **Cloudinary** - Cloud-based image storage và management
- **MailKit & MimeKit** - Email sending service
- **Redis (StackExchange.Redis)** - Distributed caching
- **ASP.NET Core Identity** - User authentication và authorization

### Security
- **JWT Bearer Authentication** - Token-based authentication
- **Cookie-based Refresh Token** - Secure token refresh mechanism
- **Role-based Authorization** - Phân quyền theo vai trò

## 📡 API Endpoints

### Authentication (`/api/Auth`)
```
POST   /api/Auth/register          - Đăng ký tài khoản mới
GET    /api/Auth/confirm-email     - Xác thực email
POST   /api/Auth/login             - Đăng nhập
POST   /api/Auth/logout            - Đăng xuất [Authorize]
POST   /api/Auth/refresh-token     - Làm mới access token
POST   /api/Auth/forgot-password   - Quên mật khẩu
POST   /api/Auth/reset-password    - Đặt lại mật khẩu
```

### User (`/api/User`)
```
GET    /api/User/profile           - Lấy thông tin profile [Authorize]
```

### Products (`/api/Product`)
```
GET    /api/Product                - Lấy danh sách products (pagination)
GET    /api/Product/{id}           - Lấy chi tiết product
POST   /api/Product                - Tạo product mới [Admin]
PATCH  /api/Product/{id}           - Cập nhật product [Admin]
PATCH  /api/Product/{id}/active    - Toggle trạng thái product [Admin]
```

### Categories (`/api/Category`)
```
GET    /api/Category               - Lấy tất cả categories
GET    /api/Category/tree          - Lấy category tree structure
POST   /api/Category               - Tạo category mới [Admin]
PATCH  /api/Category/{id}          - Cập nhật category [Admin]
DELETE /api/Category/{id}          - Xóa category (soft delete) [Admin]
```

### Cart (`/api/Cart`)
```
GET    /api/Cart                   - Lấy giỏ hàng hiện tại [Authorize]
POST   /api/Cart/items             - Thêm sản phẩm vào giỏ [Authorize]
PATCH  /api/Cart/items/{productId} - Cập nhật số lượng [Authorize]
DELETE /api/Cart/items/{productId} - Xóa sản phẩm khỏi giỏ [Authorize]
DELETE /api/Cart                   - Xóa toàn bộ giỏ hàng [Authorize]
```

### File Upload (`/api/File`)
```
POST   /api/File/avatar            - Upload avatar [Authorize]
POST   /api/File/product-image     - Upload product image [Admin]
POST   /api/File/upload/{type}     - Upload file theo loại [Authorize]
```

### Health Check (`/api/Health`)
```
GET    /api/Health                 - Kiểm tra health status [Authorize]
```

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

Services included:
- **PostgreSQL** (Port: 5432) - Main database
- **RabbitMQ** (Port: 5672, Management UI: 15672) - Message broker
- **Redis** (Port: 6379) - Distributed cache

## 📖 Project Structure

### Domain Layer
Contains business logic and entities, independent of any other layers.
- **Entities**: Product, Category, Cart, CartItem, RefreshToken
- **Identity**: ApplicationUser, ApplicationRole
- **Base Classes**: BaseEntity, BaseAuditableEntity
- **Domain Events & Exceptions**

### Application Layer
Contains use cases and business rules of the application.
- **CQRS Implementation**:
  - **Auth**: Register, Login, Logout, RefreshToken, ConfirmEmail, ForgotPassword, ResetPassword
  - **Products**: CreateProduct, UpdateProduct, ToggleActive, GetProducts (pagination), GetProductById
  - **Categories**: CreateCategory, UpdateCategory, DeleteCategory, GetAllCategories, GetCategoryTree
  - **Carts**: AddItemToCart, UpdateQuantity, ClearCart, GetCart
- **Validators**: FluentValidation cho mỗi command
- **AutoMapper Profiles**: Mapping giữa Entities và DTOs
- **Pipeline Behaviors**: ValidationBehavior
- **Background Services**: BackgroundTaskQueue cho async processing

### Infrastructure Layer
Implements interfaces defined in the Application layer.
- **DbContext**: TheGourmetDbContext với Identity integration
- **Repositories**: Generic repository pattern
- **External Services**:
  - CloudinaryService (image upload)
  - EmailService (MailKit)
  - CookieService (JWT cookie management)
- **MassTransit Integration**: RabbitMQ consumers
- **Caching**: Redis distributed cache
- **Database Seeding**: DBSeeder cho roles initialization

### API Layer
Presentation layer, handles HTTP requests.
- **Controllers**: Auth, User, Product, Category, Cart, File, Health
- **Middlewares**: GlobalExceptionMiddleware
- **Authentication**: JWT Bearer configuration

## 🔒 Security

### Authentication Flow
1. User đăng ký và nhận email xác thực
2. Sau khi xác thực, user có thể login
3. Server trả về AccessToken (JWT) và RefreshToken
4. RefreshToken được lưu trong HTTP-only cookie
5. AccessToken expire sau thời gian ngắn, sử dụng RefreshToken để gia hạn

### Authorization
- **Admin Role**: Quản lý products, categories, và các tính năng admin
- **Customer Role**: Sử dụng cart, xem products, quản lý profile

### Password Security
- Password được hash bằng ASP.NET Core Identity
- Password reset qua email với token có thời hạn
- Email confirmation bắt buộc trước khi sử dụng tài khoản

## 🤝 Contributing

All contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Standards
- Follow Clean Architecture principles
- Write unit tests for new features
- Use FluentValidation for input validation
- Implement CQRS pattern for new features
- Follow existing naming conventions

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