using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGourmet.Infrastructure.Persistence;
using TheGourmet.Domain.Entities.Identity;
using TheGourmet.Application.Interfaces;
using TheGourmet.Infrastructure.Authentication;
using TheGourmet.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TheGourmet.Infrastructure;

public static class DependencyInjection
{
    // Extension method to add infrastructure services
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) // configuration dùng để đọc dữ liệu từ appsettings.json
    {
        // set up postgre sql
        services.AddDbContext<TheGourmetDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        // setup identity 
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // configure password requirements
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true; // special characters
            options.Password.RequireUppercase = true;

            // Email is only unique identifier
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<TheGourmetDbContext>() // lưu thông tin user vào database có tên TheGourmetDbContext
        .AddDefaultTokenProviders(); // sinh ra các token như email confirmation, password reset, v.v.
        
        // setup redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "TheGourmet_";
        });

        var rsaKeyProvider = new RsaKeyProvider();

        // Register JWT services
        services.AddSingleton<IJwtKeyProvider, RsaKeyProvider>();
        services.AddScoped<ITokenService, TokenService>();
        
        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var publicKey = rsaKeyProvider.GetPublicKey();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "TheGourmet",
                ValidAudience = "TheGourmetUser",
                IssuerSigningKey = new RsaSecurityKey(publicKey),
                ClockSkew = TimeSpan.Zero // dùng UTC để tránh lỗi lệch giờ
            };
        });
        
        return services;
    }
}