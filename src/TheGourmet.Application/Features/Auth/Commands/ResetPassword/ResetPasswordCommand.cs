using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}