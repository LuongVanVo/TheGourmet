using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Vui lòng cung cấp token hợp lệ");
        }
    }
}