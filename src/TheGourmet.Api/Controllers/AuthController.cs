using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Application.Features.Auth.Commands.ConfirmEmail;
using TheGourmet.Application.Features.Auth.Commands.ForgotPassword;
using TheGourmet.Application.Features.Auth.Commands.GoogleLogin;
using TheGourmet.Application.Features.Auth.Commands.Login;
using TheGourmet.Application.Features.Auth.Commands.Logout;
using TheGourmet.Application.Features.Auth.Commands.RefreshToken;
using TheGourmet.Application.Features.Auth.Commands.Register;
using TheGourmet.Application.Features.Auth.Commands.ResetPassword;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ICookieService _cookieService;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    public AuthController(ICookieService cookieService, IMediator mediator, IConfiguration configuration)
    {
        _cookieService = cookieService;
        _mediator = mediator;
        _configuration = configuration;
    }

    // register endpoint
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    // active account endpoint
    [HttpGet("confirm-email")]
    [SwaggerOperation(Summary = "Confirm email to activate account")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    // login endpoint
    [HttpPost("login")]
    [SwaggerOperation(Summary = "User login and set auth cookies")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var loginResult = await _mediator.Send(command);

        // Set cookies
        _cookieService.SetAuthCookies(loginResult.AccessToken ?? string.Empty, loginResult.RefreshToken ?? string.Empty);

        return Ok(loginResult);
    }
    
    // login with google oauth2
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                  $"client_id={_configuration["GOOGLE_CLIENT_ID"]}&" +
                  $"response_type=code&" +
                  $"redirect_uri={_configuration["GOOGLE_REDIRECT_URI"]}&" +
                  $"scope=email%20profile%20openid&" +
                  $"access_type=offline";
        return Redirect(url);
    }
    
    // google oauth2 callback
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest("Authorization code is missing.");
        var result = await _mediator.Send(new GoogleLoginCommand
        {
            AuthorizationCode = code
        });
        
        // Set cookies
        _cookieService.SetAuthCookies(result.AccessToken ?? string.Empty, result.RefreshToken ?? string.Empty);
        return Ok(result);
    }

    // logout endpoint
    [Authorize] 
    [HttpPost("logout")]
    [SwaggerOperation(Summary = "User logout and remove auth cookies")]
    public async Task<IActionResult> Logout()
    {   
        var refreshToken = Request.Cookies["refresh_token"];
        var command = new LogoutCommand
        {
            RefreshToken = refreshToken ?? string.Empty,
        };
        var response = await _mediator.Send(command);

        // Remove cookies
        _cookieService.RemoveAuthCookies();

        return Ok(response);
    }

    // refresh token endpoint
    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh access token using refresh token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        var command = new RefreshTokenCommand
        {
            Token = refreshToken ?? string.Empty
        };

        var response = await _mediator.Send(command);
        _cookieService.SetAuthCookies(response.AccessToken ?? string.Empty, response.RefreshToken ?? string.Empty);
        return Ok(response);
    }

    [HttpGet("reset-password-page")]
    [SwaggerOperation(Summary = "Get Reset Password HTML Page")]
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
    [SwaggerOperation(Summary = "Initiate forgot password process")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Reset user password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}