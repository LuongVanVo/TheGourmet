# 🍽️ TheGourmet

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-336791?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?style=flat&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Redis](https://img.shields.io/badge/Redis-Alpine-DC382D?style=flat&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**TheGourmet** là một hệ thống backend API hiện đại, production-grade được xây dựng trên nền tảng **.NET 9**, áp dụng **Clean Architecture** và **CQRS** pattern để đảm bảo khả năng mở rộng, bảo trì và kiểm thử dễ dàng. Đây là backend của một nền tảng thương mại điện tử/giao đồ ăn gourmet với đầy đủ tính năng từ quản lý sản phẩm, giỏ hàng, đặt hàng, cho đến xử lý bất đồng bộ qua message queue.

## 📋 Overview

Dự án được thiết kế để cung cấp một REST API mạnh mẽ, tuân thủ các nguyên tắc **SOLID** và **Domain-Driven Design (DDD)**, phù hợp cho các ứng dụng quy mô lớn. Hệ thống hỗ trợ đa vai trò (Admin/Customer), quản lý catalog sản phẩm, giỏ hàng, xử lý đơn hàng, hệ thống voucher giảm giá, đánh giá sản phẩm, và tích hợp nhiều dịch vụ bên ngoài như Cloudinary, MailKit, RabbitMQ, Redis, Hangfire.

## ✨ Features

### 🔐 Authentication & Authorization
- Đăng ký tài khoản với xác thực email (email confirmation)
- Đăng nhập / đăng xuất với **JWT Bearer token**
- Cơ chế **Refresh Token** lưu trong HTTP-only cookie
- Quên mật khẩu & đặt lại mật khẩu qua email
- Phân quyền theo vai trò (**Admin**, **Customer**)
- **Google OAuth2** đăng nhập qua tài khoản Google
- Quản lý thông tin cá nhân (profile)

### 🛍️ Product Management
- CRUD đầy đủ cho sản phẩm (chỉ Admin)
- Phân trang và lọc sản phẩm theo nhiều tiêu chí
- Toggle trạng thái hoạt động (active/inactive)
- Upload ảnh sản phẩm lên **Cloudinary**
- Theo dõi số lượng tồn kho
- Hỗ trợ giá gốc và giá khuyến mãi
- Đánh giá trung bình và số lượng đánh giá
- Kiểm soát đồng thời với **RowVersion** (optimistic concurrency)

### 📂 Category Management
- CRUD đầy đủ cho danh mục (chỉ Admin)
- Cấu trúc danh mục phân cấp (Parent-Child)
- Hiển thị cây danh mục (category tree)
- **Soft delete** – không xóa dữ liệu vật lý
- Upload ảnh danh mục

### 🛒 Shopping Cart
- Thêm/xóa sản phẩm khỏi giỏ hàng
- Cập nhật số lượng sản phẩm trong giỏ
- Xóa toàn bộ giỏ hàng
- Giỏ hàng riêng biệt theo từng user
- Tính toán giá trị giỏ hàng thời gian thực

### 📋 Order Management
- Tạo đơn hàng từ giỏ hàng
- Theo dõi trạng thái đơn hàng: `Pending` → `Paid` → `Shipping` → `Completed` / `Cancelled` / `Expired`
- Xem trước đơn hàng trước khi tạo (order preview)
- Hủy đơn hàng với lý do hủy
- Xác nhận đã nhận hàng
- Tính phí vận chuyển và giảm giá
- Áp dụng **Voucher/Mã giảm giá**
- Snapshot thông tin sản phẩm tại thời điểm đặt hàng (tên, giá)
- **Tự động hủy đơn hàng** hết hạn thanh toán qua **Hangfire** scheduler

### 🎟️ Voucher & Discount System
- Giảm giá cố định (Fixed Amount)
- Giảm giá theo phần trăm (Percentage) với mức giảm tối đa
- Miễn phí vận chuyển (Free Shipping)
- Yêu cầu giá trị đơn hàng tối thiểu
- Giới hạn số lượng sử dụng
- Kiểm soát thời gian hiệu lực (start/end date)

### ⭐ Product Reviews
- Viết đánh giá sản phẩm với điểm đánh giá (1–5 sao)
- Chỉ khách hàng đã mua sản phẩm mới có thể đánh giá
- Admin có thể phản hồi đánh giá
- Tự động cập nhật điểm trung bình và số lượng đánh giá

### 📁 File Management
- Upload avatar người dùng
- Upload ảnh sản phẩm (chỉ Admin)
- **Cloudinary** cho lưu trữ ảnh trên cloud
- Phân quyền upload theo vai trò
- Upload theo loại file (avatars / products / others)

### 💌 Email & Notifications
- Gửi email xác thực tài khoản
- Gửi email đặt lại mật khẩu
- Xử lý email bất đồng bộ qua **RabbitMQ + MassTransit**
- Consumers: `SendEmailConsumer`, `ForgotPasswordConsumer`, `OrderCreatedConsumer`

### 🔍 Health Monitoring
- Endpoint kiểm tra trạng thái API
- Kiểm tra kết nối database
- Dashboard quản lý background jobs qua **Hangfire**

## 🏗️ Architecture

Dự án được tổ chức theo **Clean Architecture** với 4 tầng chính:

```
TheGourmet/
├── src/
│   ├── TheGourmet.Domain/          # Entities, Enums, Domain Events, Base Classes
│   ├── TheGourmet.Application/     # Use Cases (CQRS), DTOs, Interfaces, Validators
│   ├── TheGourmet.Infrastructure/  # Database, Repositories, External Services
│   └── TheGourmet.Api/            # Controllers, Middleware, Program.cs
├── docs/                          # Documentation
├── docker-compose.yml             # Container orchestration
├── package.json                   # Commitizen config
└── TheGourmet.sln                 # Visual Studio solution
```

### Dependency Flow
```
Api → Infrastructure → Application → Domain
```

> Tầng trong không phụ thuộc tầng ngoài. **Domain** hoàn toàn độc lập.

## 🎯 Design Patterns & Principles

### Patterns Implemented
- ✅ **Clean Architecture** – Tách biệt concerns và dependencies rõ ràng
- ✅ **CQRS (Command Query Responsibility Segregation)** – Tách read và write operations
- ✅ **Mediator Pattern** – Giao tiếp giữa các thành phần qua MediatR
- ✅ **Repository Pattern** – Abstraction cho data access
- ✅ **Unit of Work** – Quản lý transaction
- ✅ **Dependency Injection** – IoC container của .NET
- ✅ **Pipeline Behavior** – Cross-cutting concerns (validation, logging)
- ✅ **Background Task Queue** – Xử lý bất đồng bộ

### Principles
- ✅ **SOLID Principles**
- ✅ **Domain-Driven Design (DDD)**
- ✅ **Separation of Concerns**
- ✅ **Single Responsibility**

## 🚀 Technology Stack

| Thành phần | Công nghệ | Phiên bản |
|-----------|-----------|---------|
| **Framework** | .NET | 9.0 |
| **API** | ASP.NET Core Web API | 9.0 |
| **Database** | PostgreSQL | 16+ |
| **ORM** | Entity Framework Core | 9.0 |
| **Identity** | ASP.NET Core Identity | 9.0 |
| **CQRS** | MediatR | 14.0 |
| **Validation** | FluentValidation | 12.1.1 |
| **Mapping** | AutoMapper | 16.0 |
| **Message Queue** | RabbitMQ | 3.13 |
| **Messaging Framework** | MassTransit | 8.5.7 |
| **Caching** | Redis (StackExchange.Redis) | Alpine |
| **Background Jobs** | Hangfire | 1.8.22 |
| **File Storage** | Cloudinary | 1.27.9 |
| **Email** | MailKit / MimeKit | 4.14.1 |
| **Authentication** | JWT Bearer + Google OAuth2 | – |
| **DB Provider** | Npgsql | 9.0.0 |
| **API Docs** | Swagger / Swashbuckle | 10.0.1 |
| **Env Config** | DotNetEnv | 3.1.1 |

## 📡 API Endpoints

### Authentication (`/api/Auth`)
```
POST   /api/Auth/register                - Đăng ký tài khoản mới
GET    /api/Auth/confirm-email           - Xác thực email (query: email, token)
POST   /api/Auth/login                   - Đăng nhập (trả về AccessToken + RefreshToken)
GET    /api/Auth/google-login            - Chuyển hướng đăng nhập Google OAuth2
GET    /api/Auth/google-callback         - Callback sau đăng nhập Google (query: code)
POST   /api/Auth/logout                  - Đăng xuất [Authorize]
POST   /api/Auth/refresh-token           - Làm mới access token [Cookie: refresh_token]
GET    /api/Auth/reset-password-page     - Trang HTML đặt lại mật khẩu (query: email, token)
POST   /api/Auth/forgot-password         - Gửi email quên mật khẩu
POST   /api/Auth/reset-password          - Xác nhận mật khẩu mới
```

### User (`/api/User`)
```
GET    /api/User/profile                 - Lấy thông tin profile [Authorize]
```

### Products (`/api/Product`)
```
GET    /api/Product                      - Lấy danh sách sản phẩm (pagination + filter)
GET    /api/Product/{id}                 - Lấy chi tiết sản phẩm
POST   /api/Product                      - Tạo sản phẩm mới [Admin]
PATCH  /api/Product/{id}                 - Cập nhật sản phẩm [Admin]
PATCH  /api/Product/{id}/active          - Toggle trạng thái sản phẩm [Admin]
```

### Categories (`/api/Category`)
```
GET    /api/Category                     - Lấy tất cả danh mục
GET    /api/Category/tree                - Lấy cây danh mục (category tree)
POST   /api/Category                     - Tạo danh mục mới [Admin]
PATCH  /api/Category/{id}               - Cập nhật danh mục [Admin]
DELETE /api/Category/{id}               - Xóa danh mục (soft delete) [Admin]
```

### Shopping Cart (`/api/Cart`)
```
GET    /api/Cart                         - Lấy giỏ hàng hiện tại [Authorize]
POST   /api/Cart/items                   - Thêm sản phẩm vào giỏ [Authorize]
PATCH  /api/Cart/items/{productId}       - Cập nhật số lượng sản phẩm [Authorize]
DELETE /api/Cart/items/{productId}       - Xóa sản phẩm khỏi giỏ [Authorize]
DELETE /api/Cart                         - Xóa toàn bộ giỏ hàng [Authorize]
```

### Orders (`/api/Order`)
```
POST   /api/Order                        - Tạo đơn hàng từ giỏ hàng [Authorize]
GET    /api/Order/user                   - Lấy danh sách đơn hàng của user [Authorize]
POST   /api/Order/preview                - Xem trước đơn hàng trước khi tạo [Authorize]
PATCH  /api/Order/cancel/{orderId}       - Hủy đơn hàng [Authorize]
PATCH  /api/Order/confirm-receipt/{orderId} - Xác nhận đã nhận hàng [Authorize]
GET    /api/Order/cancel-reasons         - Lấy danh sách lý do hủy đơn hàng
```

### Product Reviews (`/api/ProductReview`)
```
GET    /api/ProductReview/product/{productId} - Lấy đánh giá của sản phẩm [Authorize]
POST   /api/ProductReview                - Tạo đánh giá sản phẩm [Authorize]
```

### File Upload (`/api/File`)
```
POST   /api/File/avatar                  - Upload avatar [Authorize]
POST   /api/File/product-image           - Upload ảnh sản phẩm [Admin]
POST   /api/File/upload/{type}           - Upload file theo loại [Authorize]
```

### Health Check (`/api/Health`)
```
GET    /api/Health                       - Kiểm tra health status [Authorize]
```

## 📦 Installation

### Requirements
- **.NET 9 SDK**
- **Docker & Docker Compose** (recommended – bao gồm PostgreSQL, RabbitMQ, Redis)
- PostgreSQL 16+ (nếu chạy không dùng Docker)

### Step 1: Clone repository
```bash
git clone https://github.com/LuongVanVo/TheGourmet.git
cd TheGourmet
```

### Step 2: Start Infrastructure Services
```bash
docker-compose up -d
```

Lệnh này sẽ khởi động PostgreSQL, RabbitMQ và Redis.

### Step 3: Cấu hình môi trường
Tạo hoặc chỉnh sửa file `appsettings.Development.json` trong project `TheGourmet.Api`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=thegourmet;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Issuer": "TheGourmet",
    "Audience": "TheGourmetUser",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "MailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UserName": "your-email@example.com",
    "Password": "your-password"
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### Step 4: Chạy Migration
```bash
dotnet ef database update --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

### Step 5: Chạy ứng dụng
```bash
dotnet run --project src/TheGourmet.Api
```

API sẽ chạy tại: `https://localhost:5001` (hoặc cổng được cấu hình)

## 🔧 Development

### Khôi phục dependencies
```bash
dotnet restore
```

### Build solution
```bash
dotnet build
```

### Chạy tests
```bash
dotnet test
```

### Tạo Migration mới
```bash
dotnet ef migrations add <MigrationName> --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

## 📚 API Documentation

Khi chạy ở chế độ Development, truy cập Swagger UI tại:
```
https://localhost:5001/swagger
```

### Hangfire Dashboard
Giám sát background jobs tại:
```
https://localhost:5001/hangfire
```

## 🐳 Docker

### Chạy toàn bộ stack với Docker Compose
```bash
docker-compose up -d
```

Services bao gồm:
| Service | Port | Mô tả |
|---------|------|--------|
| **PostgreSQL 15** | 5432 | Database chính |
| **RabbitMQ 3.13** | 5672 | Message broker |
| **RabbitMQ Management UI** | 15672 | Giao diện quản lý RabbitMQ |
| **Redis Alpine** | 6379 | Distributed cache |

## 📖 Project Structure

### Domain Layer (`TheGourmet.Domain`)
Chứa business entities và domain rules, **độc lập hoàn toàn** với các tầng khác.
- **Entities**: `Product`, `Category`, `Cart`, `CartItem`, `Order`, `OrderItem`, `Voucher`, `OrderCancelReason`, `ProductReview`, `RefreshToken`
- **Identity**: `ApplicationUser`, `ApplicationRole`
- **Enums**: `OrderStatus`, `DiscountType`
- **Base Classes**: `BaseEntity`, `BaseAuditableEntity`
- **Domain Events & Exceptions**

### Application Layer (`TheGourmet.Application`)
Chứa use cases và business rules của ứng dụng.
- **CQRS Implementation (Commands & Queries)**:
  - **Auth**: Register, Login, Logout, RefreshToken, ConfirmEmail, ForgotPassword, ResetPassword, GoogleLogin
  - **Products**: CreateProduct, UpdateProduct, ToggleActive, GetProducts (pagination), GetProductById
  - **Categories**: CreateCategory, UpdateCategory, DeleteCategory, GetAllCategories, GetCategoryTree
  - **Carts**: AddItemToCart, UpdateQuantity, RemoveItem, ClearCart, GetCart
  - **Orders**: CreateOrder, GetUserOrders, CancelOrder, PreviewOrder, ConfirmReceipt, GetCancelReasons
  - **ProductReviews**: CreateReview, GetProductReviews
- **Validators**: FluentValidation cho từng command
- **AutoMapper Profiles**: Mapping giữa Entities và DTOs
- **Pipeline Behaviors**: `ValidationBehavior`
- **Background Services**: `BackgroundTaskQueue` cho async processing
- **Domain Events**: Xử lý side effects sau khi command hoàn thành

### Infrastructure Layer (`TheGourmet.Infrastructure`)
Hiện thực các interfaces được định nghĩa ở Application layer.
- **DbContext**: `TheGourmetDbContext` tích hợp Identity
- **Repositories**: Generic repository pattern + Unit of Work
- **External Services**:
  - `CloudinaryService` – upload và quản lý ảnh
  - `EmailService` – gửi email qua MailKit
  - `CookieService` – quản lý JWT cookie
  - `JwtService` – tạo và xác thực JWT token
- **MassTransit / RabbitMQ Consumers**: `SendEmailConsumer`, `ForgotPasswordConsumer`, `OrderCreatedConsumer`
- **Caching**: Redis distributed cache
- **Background Jobs**: Hangfire cho tác vụ hẹn giờ (ví dụ: tự động hủy đơn hàng hết hạn)
- **Database Seeding**: `DBSeeder` cho khởi tạo roles, dữ liệu mặc định

### API Layer (`TheGourmet.Api`)
Tầng presentation, xử lý HTTP requests.
- **Controllers**: Auth, User, Product, Category, Cart, Order, ProductReview, File, Health
- **Middlewares**: `GlobalExceptionMiddleware`
- **Authentication**: JWT Bearer + Google OAuth2 configuration
- **Swagger / Hangfire** dashboard configuration

## 🔒 Security

### Authentication Flow
1. User đăng ký → nhận email xác thực → kích hoạt tài khoản
2. Đăng nhập → server trả về **AccessToken** (JWT) và **RefreshToken**
3. **RefreshToken** được lưu trong **HTTP-only cookie** (bảo mật khỏi XSS)
4. **AccessToken** ngắn hạn → dùng RefreshToken để gia hạn khi hết hạn
5. Hỗ trợ **Google OAuth2** – đăng nhập không cần mật khẩu

### Authorization
- **Admin Role**: Quản lý sản phẩm, danh mục, giám sát hệ thống
- **Customer Role**: Xem sản phẩm, quản lý giỏ hàng, đặt hàng, viết đánh giá

### Password Security
- Password được hash bởi **ASP.NET Core Identity** (bcrypt)
- Yêu cầu mật khẩu mạnh: 8+ ký tự, chữ hoa, chữ thường, số, ký tự đặc biệt
- Password reset qua email với **token có thời hạn**
- **Email confirmation** bắt buộc trước khi sử dụng tài khoản
- **Account lockout** sau nhiều lần đăng nhập thất bại

## 🤝 Contributing

Mọi đóng góp đều được chào đón! Vui lòng:
1. Fork repository này
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit các thay đổi (`git commit -m 'feat: add some AmazingFeature'`)
4. Push lên branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

### Code Standards
- Tuân theo **Clean Architecture** principles
- Viết unit tests cho tính năng mới
- Dùng **FluentValidation** cho input validation
- Áp dụng **CQRS pattern** cho tính năng mới
- Tuân theo naming conventions hiện có
- Sử dụng **Commitizen** để chuẩn hóa commit message

## 📝 License

Dự án được phát hành theo giấy phép **MIT License**.

## 👤 Author

**LuongVanVo**
- GitHub: [@LuongVanVo](https://github.com/LuongVanVo)

## 🙏 Acknowledgments

- *Clean Architecture* – Robert C. Martin
- *Domain-Driven Design* – Eric Evans
- .NET Community

---
⭐ Nếu dự án này hữu ích với bạn, hãy cho nó một ngôi sao nhé!