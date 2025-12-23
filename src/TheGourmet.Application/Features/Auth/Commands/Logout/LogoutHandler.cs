using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Auth.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Auth.Commands.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, AuthResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponse> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(command.RefreshToken);

        if (token == null)
            throw new NotFoundException("Refresh token not found");
        
        token.IsRevoked = true;
        token.IsUsed = true;
        await _refreshTokenRepository.UpdateRefreshTokenAsync(token);

        return new AuthResponse
        {
            Success = true,
            Message = "Logged out successfully"
        };
        }
    }
}