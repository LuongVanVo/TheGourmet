using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.Common.Events;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        public RegisterCommandHandler(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
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
            
            // Publish event to send email
            await _publishEndpoint.Publish(new UserRegisteredEvent
            {
                Email = request.Email,
                Subject = emailSubject,
                Body = emailBody
            });
            
            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully"
            };
        }
    }
}