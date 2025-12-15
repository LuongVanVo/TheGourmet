# 1. Tạo Solution và Folders
mkdir TheGourmet
cd TheGourmet
dotnet new sln -n TheGourmet
mkdir src

# 2. Tạo 4 Projects
dotnet new classlib -n TheGourmet.Domain -o src/TheGourmet.Domain
dotnet new classlib -n TheGourmet.Application -o src/TheGourmet.Application
dotnet new classlib -n TheGourmet.Infrastructure -o src/TheGourmet.Infrastructure
dotnet new webapi -n TheGourmet.Api -o src/TheGourmet.Api

# 3. Add vào Solution
dotnet sln add src/TheGourmet.Domain/TheGourmet.Domain.csproj
dotnet sln add src/TheGourmet.Application/TheGourmet.Application.csproj
dotnet sln add src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj
dotnet sln add src/TheGourmet.Api/TheGourmet.Api.csproj

# 4. Reference (Quan hệ phụ thuộc) - QUAN TRỌNG
dotnet add src/TheGourmet.Application/TheGourmet.Application.csproj reference src/TheGourmet.Domain/TheGourmet.Domain.csproj
dotnet add src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj reference src/TheGourmet.Application/TheGourmet.Application.csproj
dotnet add src/TheGourmet.Api/TheGourmet.Api.csproj reference src/TheGourmet.Application/TheGourmet.Application.csproj
dotnet add src/TheGourmet.Api/TheGourmet.Api.csproj reference src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj

# 5. Cài NuGet Packages cần thiết
# Application Layer
dotnet add src/TheGourmet.Application/TheGourmet.Application.csproj package MediatR
dotnet add src/TheGourmet.Application/TheGourmet.Application.csproj package FluentValidation
dotnet add src/TheGourmet.Application/TheGourmet.Application.csproj package AutoMapper

# Infrastructure Layer
dotnet add src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools
dotnet add src/TheGourmet.Infrastructure/TheGourmet.Infrastructure.csproj package MassTransit.RabbitMQ

# Api Layer
dotnet add src/TheGourmet.Api/TheGourmet.Api.csproj package Microsoft.EntityFrameworkCore.Design