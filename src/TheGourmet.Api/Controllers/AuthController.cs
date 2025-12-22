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

    [HttpGet("reset-password-page")]
    public IActionResult ResetPasswordPage([FromQuery] string email, [FromQuery] string token)
    {       
        var htmlContent = $@"
        <html>
        <head>
            <title>Reset Password</title>
            <style>
                body {{ font-family: Arial, sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; background-color: #f0f2f5; }}
                .card {{ background: white; padding: 2rem; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); width: 100%; max-width: 400px; }}
                input {{ width: 100%; padding: 10px; margin: 10px 0; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box; }}
                button {{ width: 100%; padding: 10px; background-color: #4CAF50; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; }}
                button:hover {{ background-color: #45a049; }}
                .message {{ margin-bottom: 15px; color: #333; text-align: center; }}
            </style>
        </head>
        <body>
            <div class='card'>
                <h2 style='text-align:center'>The Gourmet</h2>
                <p class='message'>Reset password for account: <b>{email}</b></p>
                
                <form id='resetForm'>
                    <input type='hidden' id='email' value='{email}' />
                    <input type='hidden' id='token' value='{token}' />
                    
                    <label>New Password:</label>
                    <input type='password' id='newPassword' required placeholder='Enter new password...' />
                    
                    <label>Confirm Password:</label>
                    <input type='password' id='confirmPassword' required placeholder='Re-enter password...' />
                    
                    <button type='submit'>Change Password</button>
                </form>

                <p id='status' style='text-align:center; margin-top:10px; color:red;'></p>
            </div>

            <script>
                document.getElementById('resetForm').addEventListener('submit', async function(e) {{
                    e.preventDefault();
                    const pass = document.getElementById('newPassword').value;
                    const confirm = document.getElementById('confirmPassword').value;
                    const status = document.getElementById('status');

                    if(pass !== confirm) {{
                        status.textContent = 'Password confirmation does not match!';
                        return;
                    }}

                    const data = {{
                        email: document.getElementById('email').value,
                        token: document.getElementById('token').value,
                        newPassword: pass
                    }};

                    try {{
                        // Gọi chính API Reset Password của Backend
                        const response = await fetch('/api/auth/reset-password', {{
                            method: 'POST',
                            headers: {{ 'Content-Type': 'application/json' }},
                            body: JSON.stringify(data)
                        }});

                        const result = await response.json();
                        
                        if(response.ok) {{
                            document.body.innerHTML = '<h2 style=\'text-align:center; color:green\'>Password changed successfully! You can close this tab now.</h2>';
                        }} else {{
                            status.textContent = result.message || 'An error occurred.';
                        }}
                    }} catch (error) {{
                        status.textContent = 'Server connection error.';
                    }}
                }});
            </script>
        </body>
        </html>";

        // Trả về nội dung HTML
        return Content(htmlContent, "text/html");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await _authService.ForgotPasswordAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var response = await _authService.ResetPasswordAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}