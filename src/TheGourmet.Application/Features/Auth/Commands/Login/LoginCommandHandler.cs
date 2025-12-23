using MediatR;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using RefreshTokenEntity = TheGourmet.Domain.Entities.Identity.RefreshToken;

namespace TheGourmet.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            // check if user exists
        var foundUser = await _userRepository.GetUserByEmailAsync(command.Email);
        if (foundUser == null) 
            throw new NotFoundException("User not found");
        
        // check account is active ?
        if (!foundUser.IsActive) 
            throw new BadRequestException("Account is not activated. Please check your email to activate your account.");
        
        // check password
        var isPasswordValid = await _userRepository.CheckPasswordAsync(foundUser, command.Password);
        if (!isPasswordValid) 
            throw new BadRequestException("Invalid password or email! Please try again.");
        
        // get user roles
        var roles = await _userRepository.GetUserRolesAsync(foundUser);

        // generate tokens
        var (accessToken, jwtId) = _tokenService.GenerateAccessToken(foundUser, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // save refresh token to database
        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            JwtId = jwtId,
            IsUsed = false,
            IsRevoked = false,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenValidityInDays"] ?? "7")),
            UserId = foundUser.Id
        };

        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,    
        };
        }
    }
}