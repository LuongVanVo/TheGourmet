using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}