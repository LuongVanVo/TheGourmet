using FluentValidation;

namespace TheGourmet.Application.Features.Vouchers.Commands.UpdateVoucher;

public class UpdateVoucherValidator : AbstractValidator<UpdateVoucherCommand>
{
    public UpdateVoucherValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Voucher Id should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("Voucher Id must be a valid GUID.");
        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0).When(x => x.DiscountType == Domain.Enums.DiscountType.Percentage)
            .WithMessage("Max discount amount must be greater than 0 for percentage discounts.");
        RuleFor(x => x.DiscountType)
            .IsInEnum().WithMessage("Invalid discount type.");
    }
}