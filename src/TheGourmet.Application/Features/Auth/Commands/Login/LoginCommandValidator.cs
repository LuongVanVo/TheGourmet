using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Vui lòng nhập Email")
                .EmailAddress().WithMessage("Email không hợp lệ");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu");
        }
    }
}