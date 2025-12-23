using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User không hợp lệ.");
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token không hợp lệ.");
        }
    }
}