using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using TheGourmet.Api.Middlewares;
using TheGourmet.Application;
using TheGourmet.Application.Interfaces;
using TheGourmet.Domain.Entities.Identity;
using TheGourmet.Infrastructure;
using TheGourmet.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect Layer Application
builder.Services.AddApplicationServices();
// Connect Layer Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Hangfire setup
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

// add swagger details
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

// Add Exception Handling Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire"); // access via http://localhost:xxxx/hangfire
app.MapControllers();

app.Run();