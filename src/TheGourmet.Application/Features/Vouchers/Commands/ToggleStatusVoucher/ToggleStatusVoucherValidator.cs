using FluentValidation;

namespace TheGourmet.Application.Features.Vouchers.Commands.ToggleStatusVoucher;

public class ToggleStatusVoucherValidator : AbstractValidator<ToggleStatusVoucherCommand>
{
    public ToggleStatusVoucherValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Voucher Id should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("Voucher Id must be a valid GUID.");
    }
}