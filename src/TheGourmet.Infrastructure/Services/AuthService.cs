namespace TheGourmet.Infrastructure.Services;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.DTOs.Auth;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities.Identity;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    // DI UserManager of IdentityUser and TokenService
    public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager, IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _userRepository = userRepository;  
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // check if user already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null) 
            throw new BadRequestException("User with this email already exists");

        var newUser = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            Fullname = request.Fullname,
            IsActive = false
        };

        var result = await _userRepository.CreateUserAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"User registration failed: {errors}");
        }

        // Assign "Customer" role to new user
        await _userManager.AddToRoleAsync(newUser, "Customer");

        // Create and Send email confirm to active account
        var verificationToken = await _userRepository.GenerateEmailConfirmationTokenAsync(newUser);
        if (verificationToken == null)
        {
            throw new BadRequestException("Failed to generate email confirmation token");
        }

        var vertificationLink = $"{_configuration["ClientUrl"]}/api/auth/confirm-email?userId={newUser.Id}&token={Uri.EscapeDataString(verificationToken)}";
        var emailSubject = "Xác thực tài khoản TheGourmet";
        var emailBody = $@"
            <html>
                <body>
                    <h2>Chào {request.Fullname},</h2>
                    <p>Cảm ơn bạn đã đăng ký tài khoản tại The Gourmet.</p>
                    <p>Vui lòng click vào nút bên dưới để kích hoạt tài khoản:</p>
                    <a href='{vertificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none;'>Kích hoạt ngay</a>
                </body>
            </html>";
        
        // Tạo luồng riêng gửi email để không làm chậm phản hồi API
        _ = Task.Run(() => _emailService.SendEmailAsync(request.Email, emailSubject, emailBody));

        return new AuthResponse
        {
            Success = true,
            Message = "User registered successfully"
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // check if user exists
        var foundUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (foundUser == null) 
            throw new NotFoundException("User not found");
        
        // check account is active ?
        if (!foundUser.IsActive) 
            throw new BadRequestException("Account is not activated. Please check your email to activate your account.");
        
        // check password
        var isPasswordValid = await _userRepository.CheckPasswordAsync(foundUser, request.Password);
        if (!isPasswordValid) 
            throw new BadRequestException("Invalid password or email! Please try again.");
        
        // get user roles
        var roles = await _userRepository.GetUserRolesAsync(foundUser);

        // generate tokens
        var accessToken = _tokenService.GenerateAccessToken(foundUser, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,    
        };
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userRepository.GetUserByUserId(userId);
        if (user == null) 
            throw new NotFoundException("User not found");
        
        // check token used
        if (user.IsActive) throw new BadRequestException("Account has already been confirmed");
        var result = await _userRepository.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Email confirmation failed: {errors}");
        }

        user.IsActive = true;

        await _userRepository.UpdateUserAsync(user);

        return result.Succeeded;
    }
}