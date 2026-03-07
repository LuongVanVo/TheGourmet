# 🍽️ TheGourmet

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?style=flat&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Redis](https://img.shields.io/badge/Redis-Alpine-DC382D?style=flat&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**TheGourmet** is a modern, production-grade backend API system built on **.NET 9**, applying **Clean Architecture** and **CQRS** patterns to ensure scalability, maintainability, and testability. It serves as the backend for a gourmet food e-commerce / delivery platform, covering everything from product management, shopping cart, and order processing to asynchronous messaging via a message queue.

## 📋 Overview

The project is designed to provide a robust REST API that adheres to **SOLID** principles and **Domain-Driven Design (DDD)**, making it suitable for large-scale applications. The system supports multiple roles (Admin/Customer), product catalog management, shopping cart, order processing, a voucher/discount system, product reviews, and integrations with external services such as Cloudinary, MailKit, RabbitMQ, Redis, and Hangfire.

## ✨ Features

### 🔐 Authentication & Authorization
- User registration with email confirmation
- Login / logout using **JWT Bearer token**
- **Refresh Token** mechanism stored in an HTTP-only cookie
- Forgot password & password reset via email
- Role-based authorization (**Admin**, **Customer**)
- **Google OAuth2** sign-in support
- User profile management

### 🛍️ Product Management
- Full CRUD for products (Admin only)
- Pagination and multi-criteria product filtering
- Toggle active/inactive status
- Product image upload to **Cloudinary**
- Inventory stock tracking
- Support for original and promotional pricing
- Average rating and review count
- Optimistic concurrency control with **RowVersion**

### 📂 Category Management
- Full CRUD for categories (Admin only)
- Hierarchical category structure (Parent-Child)
- Category tree view
- **Soft delete** – no physical data removal
- Category image upload

### 🛒 Shopping Cart
- Add/remove products from cart
- Update product quantities in cart
- Clear entire cart
- Per-user isolated shopping carts
- Real-time cart total calculation

### 📋 Order Management
- Create orders from shopping cart
- Order status tracking: `Pending` → `Paid` → `Shipping` → `Completed` / `Cancelled` / `Expired`
- Order preview before placement
- Cancel order with cancellation reason
- Confirm receipt of order
- Shipping fee and discount calculation
- Apply **Voucher / Discount codes**
- Snapshot of product info at order time (name, price)
- **Automatic expiration & cancellation** of unpaid orders via **Hangfire** scheduler

### 🎟️ Voucher & Discount System
- Fixed amount discount
- Percentage discount with maximum discount cap
- Free shipping discount
- Minimum order value requirement
- Usage limit control
- Active date range (start/end date)
- Toggle voucher active status
- Admin-only management (create, update, toggle status)

### ⭐ Product Reviews
- Write product reviews with star ratings (1–5)
- Only customers who have purchased the product can leave a review
- Admin can reply to reviews
- Automatically updates average rating and review count

### 📁 File Management
- Upload user avatars
- Upload product images (Admin only)
- **Cloudinary** for cloud-based image storage
- Role-based upload permissions
- Upload by file type (avatars / products / others)

### 💌 Email & Notifications
- Send account verification emails
- Send password reset emails
- Asynchronous email processing via **RabbitMQ + MassTransit**
- Consumers: `SendEmailConsumer`, `ForgotPasswordConsumer`, `OrderCreatedConsumer`

### 🔍 Health Monitoring
- API health status endpoint
- Database connectivity check
- Background job management dashboard via **Hangfire**

## 🏗️ Architecture

The project follows **Clean Architecture** with 4 main layers:

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

> Inner layers do not depend on outer layers. The **Domain** layer is completely independent.

## 🎯 Design Patterns & Principles

### Patterns Implemented
- ✅ **Clean Architecture** – Clear separation of concerns and dependencies
- ✅ **CQRS (Command Query Responsibility Segregation)** – Separate read and write operations
- ✅ **Mediator Pattern** – Component communication via MediatR
- ✅ **Repository Pattern** – Abstraction for data access
- ✅ **Unit of Work** – Transaction management
- ✅ **Dependency Injection** – .NET IoC container
- ✅ **Pipeline Behavior** – Cross-cutting concerns (validation, logging)
- ✅ **Background Task Queue** – Asynchronous processing

### Principles
- ✅ **SOLID Principles**
- ✅ **Domain-Driven Design (DDD)**
- ✅ **Separation of Concerns**
- ✅ **Single Responsibility**

## 🚀 Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **Framework** | .NET | 9.0 |
| **API** | ASP.NET Core Web API | 9.0 |
| **Database** | PostgreSQL | 15+ |
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
POST   /api/Auth/register                - Register a new account
GET    /api/Auth/confirm-email           - Confirm email address (query: email, token)
POST   /api/Auth/login                   - Login (returns AccessToken + RefreshToken)
GET    /api/Auth/google-login            - Redirect to Google OAuth2 login
GET    /api/Auth/google-callback         - Callback after Google login (query: code)
POST   /api/Auth/logout                  - Logout [Authorize]
POST   /api/Auth/refresh-token           - Refresh access token [Cookie: refresh_token]
GET    /api/Auth/reset-password-page     - HTML page for password reset (query: email, token)
POST   /api/Auth/forgot-password         - Send forgot password email
POST   /api/Auth/reset-password          - Confirm new password
```

### User (`/api/User`)
```
GET    /api/User/profile                 - Get user profile [Authorize]
```

### Products (`/api/Product`)
```
GET    /api/Product                      - Get product list (pagination + filter)
GET    /api/Product/{id}                 - Get product details
POST   /api/Product                      - Create new product [Admin]
PATCH  /api/Product/{id}                 - Update product [Admin]
PATCH  /api/Product/{id}/active          - Toggle product status [Admin]
```

### Categories (`/api/Category`)
```
GET    /api/Category                     - Get all categories
GET    /api/Category/tree                - Get category tree
POST   /api/Category                     - Create new category [Admin]
PATCH  /api/Category/{id}               - Update category [Admin]
DELETE /api/Category/{id}               - Delete category (soft delete) [Admin]
```

### Shopping Cart (`/api/Cart`)
```
GET    /api/Cart                         - Get current cart [Authorize]
POST   /api/Cart/items                   - Add item to cart [Authorize]
PATCH  /api/Cart/items/{productId}       - Update item quantity [Authorize]
DELETE /api/Cart/items/{productId}       - Remove item from cart [Authorize]
DELETE /api/Cart                         - Clear entire cart [Authorize]
```

### Orders (`/api/Order`)
```
POST   /api/Order                        - Create order from cart [Authorize]
GET    /api/Order/user                   - Get user's orders [Authorize]
POST   /api/Order/preview                - Preview order before placing [Authorize]
PATCH  /api/Order/cancel/{orderId}       - Cancel order [Authorize]
PATCH  /api/Order/confirm-receipt/{orderId} - Confirm order received [Authorize]
GET    /api/Order/cancel-reasons         - Get list of cancellation reasons
```

### Vouchers (`/api/Voucher`)
```
POST   /api/Voucher                      - Create new voucher [Admin]
PATCH  /api/Voucher                      - Update voucher [Admin]
PATCH  /api/Voucher/toggle-status        - Toggle voucher status [Admin]
GET    /api/Voucher                      - Get vouchers with pagination and search [Authorize]
```

### Product Reviews (`/api/ProductReview`)
```
GET    /api/ProductReview/product/{productId} - Get product reviews [Authorize]
POST   /api/ProductReview                - Create product review [Authorize]
```

### File Upload (`/api/File`)
```
POST   /api/File/avatar                  - Upload avatar [Authorize]
POST   /api/File/product-image           - Upload product image [Admin]
POST   /api/File/upload/{type}           - Upload file by type [Authorize]
```

### Health Check (`/api/Health`)
```
GET    /api/Health                       - Check health status [Authorize]
```

## 📦 Installation

### Requirements
- **.NET 9 SDK**
- **Docker & Docker Compose** (recommended – includes PostgreSQL, RabbitMQ, Redis)
- PostgreSQL 15+ (if running without Docker)

### Step 1: Clone the repository
```bash
git clone https://github.com/LuongVanVo/TheGourmet.git
cd TheGourmet
```

### Step 2: Start Infrastructure Services
```bash
docker-compose up -d
```

This will start PostgreSQL, RabbitMQ, and Redis.

### Step 3: Configure Environment
Create or edit `appsettings.Development.json` inside the `TheGourmet.Api` project:
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

### Step 4: Apply Migrations
```bash
dotnet ef database update --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

### Step 5: Run the Application
```bash
dotnet run --project src/TheGourmet.Api
```

The API will be available at: `https://localhost:5001` (or the configured port)

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

### Create a new Migration
```bash
dotnet ef migrations add <MigrationName> --project src/TheGourmet.Infrastructure --startup-project src/TheGourmet.Api
```

## 📚 API Documentation

When running in Development mode, access Swagger UI at:
```
https://localhost:5001/swagger
```

### Hangfire Dashboard
Monitor background jobs at:
```
https://localhost:5001/hangfire
```

## 🐳 Docker

### Run the full stack with Docker Compose
```bash
docker-compose up -d
```

Services included:
| Service | Port | Description |
|---------|------|-------------|
| **PostgreSQL 15** | 5433 | Primary database |
| **RabbitMQ 3.13** | 5672 | Message broker |
| **RabbitMQ Management UI** | 15672 | RabbitMQ management interface |
| **Redis Alpine** | 6380 | Distributed cache |

## 📖 Project Structure

### Domain Layer (`TheGourmet.Domain`)
Contains business entities and domain rules, **completely independent** from all other layers.
- **Entities**: `Product`, `Category`, `Cart`, `CartItem`, `Order`, `OrderItem`, `Voucher`, `OrderCancelReason`, `ProductReview`, `RefreshToken`
- **Identity**: `ApplicationUser`, `ApplicationRole`
- **Enums**: `OrderStatus`, `DiscountType`
- **Base Classes**: `BaseEntity`, `BaseAuditableEntity`
- **Domain Events & Exceptions**

### Application Layer (`TheGourmet.Application`)
Contains use cases and application business rules.
- **CQRS Implementation (Commands & Queries)**:
  - **Auth**: Register, Login, Logout, RefreshToken, ConfirmEmail, ForgotPassword, ResetPassword, GoogleLogin
  - **Products**: CreateProduct, UpdateProduct, ToggleActive, GetProducts (pagination), GetProductById
  - **Categories**: CreateCategory, UpdateCategory, DeleteCategory, GetAllCategories, GetCategoryTree
  - **Carts**: AddItemToCart, UpdateQuantity, RemoveItem, ClearCart, GetCart
  - **Orders**: CreateOrder, GetUserOrders, CancelOrder, PreviewOrder, ConfirmReceipt, GetCancelReasons
  - **Vouchers**: CreateVoucher, UpdateVoucher, ToggleStatusVoucher, GetVouchers
  - **ProductReviews**: CreateReview, GetProductReviews
- **Validators**: FluentValidation for each command
- **AutoMapper Profiles**: Mapping between Entities and DTOs
- **Pipeline Behaviors**: `ValidationBehavior`
- **Background Services**: `BackgroundTaskQueue` for async processing
- **Domain Events**: Handle side effects after commands complete

### Infrastructure Layer (`TheGourmet.Infrastructure`)
Implements interfaces defined in the Application layer.
- **DbContext**: `TheGourmetDbContext` integrated with Identity
- **Repositories**: Generic repository pattern + Unit of Work
- **External Services**:
  - `CloudinaryService` – image upload and management
  - `EmailService` – email sending via MailKit
  - `CookieService` – JWT cookie management
  - `JwtService` – JWT token creation and validation
- **MassTransit / RabbitMQ Consumers**: `SendEmailConsumer`, `ForgotPasswordConsumer`, `OrderCreatedConsumer`
- **Caching**: Redis distributed cache
- **Background Jobs**: Hangfire for scheduled tasks (e.g., auto-cancel expired orders)
- **Database Seeding**: `DBSeeder` for initializing roles and default data

### API Layer (`TheGourmet.Api`)
Presentation layer that handles HTTP requests.
- **Controllers**: Auth, User, Product, Category, Cart, Order, Voucher, ProductReview, File, Health
- **Middlewares**: `GlobalExceptionMiddleware`
- **Authentication**: JWT Bearer + Google OAuth2 configuration
- **Swagger / Hangfire** dashboard configuration

## 🔒 Security

### Authentication Flow
1. User registers → receives verification email → activates account
2. Login → server returns **AccessToken** (JWT) and **RefreshToken**
3. **RefreshToken** is stored in an **HTTP-only cookie** (protected from XSS)
4. **AccessToken** is short-lived → use RefreshToken to renew when expired
5. **Google OAuth2** supported – sign in without a password

### Authorization
- **Admin Role**: Manage products, categories, vouchers, and monitor system
- **Customer Role**: Browse products, manage cart, place orders, write reviews

### Password Security
- Passwords are hashed by **ASP.NET Core Identity** (bcrypt)
- Strong password requirements: 8+ characters, uppercase, lowercase, digits, special characters
- Password reset via email with a **time-limited token**
- **Email confirmation** required before account use
- **Account lockout** after multiple failed login attempts

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'feat: add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Standards
- Follow **Clean Architecture** principles
- Write unit tests for new features
- Use **FluentValidation** for input validation
- Apply the **CQRS pattern** for new features
- Follow existing naming conventions
- Use **Commitizen** to standardize commit messages

## 📝 License

This project is released under the **MIT License**.

## 👤 Author

**LuongVanVo**
- GitHub: [@LuongVanVo](https://github.com/LuongVanVo)

## 🙏 Acknowledgments

- *Clean Architecture* – Robert C. Martin
- *Domain-Driven Design* – Eric Evans
- .NET Community

---
⭐ If you find this project helpful, please give it a star!