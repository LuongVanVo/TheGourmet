using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using TheGourmet.Application.Features.Auth.Results;

namespace TheGourmet.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = string.Empty;
    }
}