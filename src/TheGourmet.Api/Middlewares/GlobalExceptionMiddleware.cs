using FluentValidation;
using System.Net;
using System.Text.Json;
using TheGourmet.Application.Exceptions;

namespace TheGourmet.Api.Middlewares
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new { message = "", errors = new Dictionary<string, string[]>() };

            switch (exception)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    
                    logger.LogWarning("Validation failed: {@Errors}", errors);
                    
                    response = new { message = "Validation failed", errors };
                    break;

                case BadRequestException badRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    logger.LogWarning("Bad request: {Message}", badRequestException.Message);
                    response = new { message = badRequestException.Message, errors = new Dictionary<string, string[]>() };
                    break;
                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    logger.LogWarning("Not found: {Message}", notFoundException.Message);
                    response = new { message = notFoundException.Message, errors = new Dictionary<string, string[]>() };
                    break;
                case ForbiddenException forbiddenException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    logger.LogWarning("Forbidden: {Message}", forbiddenException.Message);
                    response = new { message = forbiddenException.Message, errors = new Dictionary<string, string[]>() };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                    response = new { message = "An error occurred while processing your request", errors = new Dictionary<string, string[]>() };
                    break;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}