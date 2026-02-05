using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<AuthResponse>
    {
        public string AuthorizationCode { get; set; } = string.Empty;
    }
}