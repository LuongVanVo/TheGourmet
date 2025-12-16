using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheGourmet.Infrastructure.Persistence;
using TheGourmet.Domain.Entities.Identity;


namespace TheGourmet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
        .AddEntityFrameworkStores<TheGourmetDbContext>()
        .AddDefaultTokenProviders();
        
        // setup redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "TheGourmet_";
        });
        
        return services;
    }
}