using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.Logout
{
    public class LogoutValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Vui lòng cung cấp refresh token hợp lệ");
        }
    }
}