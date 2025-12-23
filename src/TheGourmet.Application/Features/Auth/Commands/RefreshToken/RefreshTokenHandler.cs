using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces;
using TheGourmet.Application.Interfaces.Repositories;
using RefreshTokenEntity = TheGourmet.Domain.Entities.Identity.RefreshToken;

namespace TheGourmet.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public RefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IUserRepository userRepository, IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            // 1. Find refresh token in database
        var storedToken = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(command.Token);

        if (storedToken == null)
            throw new NotFoundException("Refresh token not found");
        
        if (storedToken.IsUsed) 
            throw new BadRequestException("Refresh token has been used");
        
        if (storedToken.IsRevoked) 
            throw new BadRequestException("Refresh token has been revoked");
        
        if (storedToken.ExpiryDate < DateTime.UtcNow) 
            throw new BadRequestException("Refresh token has expired");
        
        // 2. Mark token as used
        storedToken.IsUsed = true;
        storedToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateRefreshTokenAsync(storedToken);

        // 3. Release new tokens
        var user = storedToken.User;
        var roles = await _userRepository.GetUserRolesAsync(user);

        var (newAccessToken, newJwtId) = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // 5. Save new refresh token to database
        var newRefreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            JwtId = newJwtId,
            IsUsed = false,
            IsRevoked = false,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenValidityInDays"] ?? "7")),
            UserId = user.Id
        };

        await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
        
        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };
        }
    }
}