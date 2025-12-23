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
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Infrastructure.Persistence.Repositories;
using TheGourmet.Application.Common;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using MassTransit;
using TheGourmet.Infrastructure.Consumer;

namespace TheGourmet.Infrastructure;

public static class DependencyInjection
{
    // Extension method to add infrastructure services
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) // configuration dùng để đọc dữ liệu từ appsettings.json
    {
        // set up postgre sql
        services.AddDbContextPool<TheGourmetDbContext>(options =>
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
            options.Password.RequireLowercase = true;

            // Email is only unique identifier
            options.User.RequireUniqueEmail = true;

            // Lockout settings - để chống brute force attack
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<TheGourmetDbContext>() // lưu thông tin user vào database có tên TheGourmetDbContext
        .AddDefaultTokenProviders(); // sinh ra các token như email confirmation, password reset, v.v.
        
        // Configure Section
        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 10000;
        });

        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MailSettings>>().Value);

        // config RabbitMQ
        services.AddMassTransit(x =>
        {
            // Đăng ký Consumer để MassTransit biết class nào xử lý tin nhắn nào 
            x.AddConsumer<SendEmailConsumer>();
            x.AddConsumer<ForgotPasswordConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                // Kết nối đến RabbitMQ Docker
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                // Tự động đặt tên Queue và gán Consumer vào Queue đó
                cfg.ConfigureEndpoints(context);
            });
        });
        // setup redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "TheGourmet_";
        });

        // setup RSA keys
        var rsaKeyProvider = new RsaKeyProvider();
        var publicKey = rsaKeyProvider.GetPublicKey();

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

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new RsaSecurityKey(publicKey),
                ClockSkew = TimeSpan.Zero // dùng UTC để tránh lỗi lệch giờ
            };

        // Read Token From Cookie (HttpOnly)
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("access_token"))
                    {
                        // Get token from cookie push to context to .NET validate
                        context.Token = context.Request.Cookies["access_token"];
                    }
                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    context.HandleResponse(); // ngăn chặn hành vi mặc định (như redirect login page)

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "You are not logged in or the login session has expired. Please log in again.",
                        errorCode = "UNAUTHORIZED",
                    });

                    return context.Response.WriteAsync(result);
                },

                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new 
                    { 
                        success = false,
                        message = "You do not have access to this resource.",
                        errorCode = "FORBIDDEN"
                    });

                    return context.Response.WriteAsync(result);
                }
            };
        });
        
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        // Register Email Service
        services.AddTransient<IEmailService, EmailService>(); // Transient vì service này không giữ trạng thái, mỗi lần gọi tạo một instance mới

        // Register Cookie Service
        services.AddHttpContextAccessor(); // Đăng ký để truy cập HTTP Context ngoài Controller
        services.AddScoped<ICookieService, CookieService>();

        // Register RefreshToken Repository
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        
        return services;
    }
}