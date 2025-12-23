using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Vui lòng nhập Email")
                .EmailAddress().WithMessage("Email không hợp lệ");
            
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token không được để trống");
            
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu mới");
        }
    }
}