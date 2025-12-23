using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string Token { get; set; } = string.Empty;
    }
}