using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using TheGourmet.Application.Common.Behaviors;
using MediatR;

namespace TheGourmet.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Lấy thông tin của toàn bộ project hiện tại
            var assembly = Assembly.GetExecutingAssembly();

            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Register Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}