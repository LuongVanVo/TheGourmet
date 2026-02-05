using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Api.Helper;
using TheGourmet.Infrastructure.Persistence;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly TheGourmetDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(TheGourmetDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    [SwaggerOperation(Summary = "Check the health status of TheGourmet API")]
    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            // check connect of Database
            bool dbConnection = await _context.Database.CanConnectAsync();

            if (dbConnection)
            {
                return Ok(new
                {
                    Status = "Healthy",
                    Message = "TheGourmet API is healthy.",
                    Timestamp = DateTime.UtcNow,
                    User = User.GetCurrentUserId()
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Message = $"TheGourmet API is unhealthy. Error: {ex.Message}",
                Timestamp = DateTime.UtcNow
            });
        }

        return StatusCode(500, new
        {
            Status = "Unhealthy",
            Message = "TheGourmet API is unhealthy. Cannot connect to the database.",
            Timestamp = DateTime.UtcNow
        });
    }
}