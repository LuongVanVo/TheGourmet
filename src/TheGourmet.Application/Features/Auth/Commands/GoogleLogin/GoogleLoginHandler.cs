using MediatR;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities.Identity;
using RefreshTokenEntity = TheGourmet.Domain.Entities.Identity.RefreshToken;

namespace TheGourmet.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IGoogleAuthenticationService googleAuthenticationService,
        ITokenService tokenService,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository)
        : IRequestHandler<GoogleLoginCommand, AuthResponse>
    {
        public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            // get user info from google
            var googleInfo = await googleAuthenticationService.ValidateTokenAndGetUserInfoAsync(request.AuthorizationCode);

            var user = await userRepository.GetUserByEmailAsync(googleInfo.Email);
            if (user == null)
            {
                // register new user
                user = new ApplicationUser
                {
                    Email = googleInfo.Email,
                    UserName = googleInfo.Email,
                    Fullname = googleInfo.Name,
                    AvatarUrl = googleInfo.Picture,
                    IsGoogleAccount = true,
                    IsActive = true,
                    PasswordHash = null,
                };
                var result = await userRepository.CreateUserAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"User registration failed: {errors}");
                }
            }
            else
            {
                // existing user - login
                if (!user.IsActive)
                {
                    throw new Exception("Account is not activated. Please check your email to activate your account.");
                }

                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = googleInfo.Picture;
                    await userRepository.UpdateUserAsync(user);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var roles = await userRepository.GetUserRolesAsync(user);
            var (accessToken, jwtId) = tokenService.GenerateAccessToken(user, roles);
            var refreshToken = tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshTokenEntity
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                JwtId = jwtId,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate =
                    DateTime.UtcNow.AddDays(int.Parse(configuration["JwtSettings:RefreshTokenValidityInDays"] ?? "7")),
                UserId = user.Id
            };
            await refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);

            return new AuthResponse
            {
                Success = true,
                Message = "Google login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}