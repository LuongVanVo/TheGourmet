using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using TheGourmet.Api.Middlewares;
using TheGourmet.Application.Interfaces;
using TheGourmet.Domain.Entities.Identity;
using TheGourmet.Infrastructure;
using TheGourmet.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect Layer Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);


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
app.MapControllers();

app.Run();