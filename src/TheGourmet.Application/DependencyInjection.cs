using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using TheGourmet.Application.Common.Behaviors;
using MediatR;
using TheGourmet.Application.Common.ExternalServices;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Lấy thông tin của toàn bộ project hiện tại
            var assembly = Assembly.GetExecutingAssembly();
            
            // Register AutoMapper
            services.AddAutoMapper(_ => { }, assembly);
            
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Register Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            // Register Background Task 
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            return services;
        }
    }
}