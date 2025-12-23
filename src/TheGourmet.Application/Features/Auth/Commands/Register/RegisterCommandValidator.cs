using FluentValidation;

namespace TheGourmet.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Fullname).NotEmpty().WithMessage("Tên không được để trống");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email không hợp lệ");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Mật khẩu xác nhận không khớp");
        }
    }
}