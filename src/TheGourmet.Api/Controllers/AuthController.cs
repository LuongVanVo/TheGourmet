using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.DTOs.Auth;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly ICookieService _cookieService;

    public AuthController(IAuthService authService, ILogger<AuthController> logger, ICookieService cookieService)
    {
        _authService = authService;
        _logger = logger;
        _cookieService = cookieService;
    }

    // register endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    // active account endpoint
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("UserId and token are required");
        }

        var response = await _authService.ConfirmEmailAsync(userId, token);
        return Ok(response);
    }

    // login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginResult = await _authService.LoginAsync(request);

        // Set cookies
        _cookieService.SetAuthCookies(loginResult.AccessToken ?? string.Empty, loginResult.RefreshToken ?? string.Empty);

        return Ok(loginResult);
    }

    // logout endpoint
    [Authorize] 
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {   
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest(new { Message = "Refresh token is missing" });
        }
        var response = await _authService.LogoutAsync(refreshToken);

        // Remove cookies
        _cookieService.RemoveAuthCookies();

        return Ok(response);
    }

    // refresh token endpoint
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Message = "Refresh token is missing" });
        }

        var response = await _authService.RefreshTokenAsync(refreshToken);
        _cookieService.SetAuthCookies(response.AccessToken ?? string.Empty, response.RefreshToken ?? string.Empty);
        return Ok(response);
    }
}