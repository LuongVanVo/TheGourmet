using FluentValidation;

namespace TheGourmet.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherValidator : AbstractValidator<CreateVoucherCommand>
{
    public CreateVoucherValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code should not be empty")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");
        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than 0.");
        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100).When(x => x.DiscountType == Domain.Enums.DiscountType.Percentage)
            .WithMessage("Discount value must be between 0 and 100 for percentage discounts.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");
        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0).When(x => x.DiscountType == Domain.Enums.DiscountType.Percentage)
            .WithMessage("Max discount amount must be greater than 0 for percentage discounts.");
        RuleFor(x => x.MaxDiscountAmount)
            .Must(x => x == null || x == 0)
            .When(x => x.DiscountType != Domain.Enums.DiscountType.Percentage)
            .WithMessage("Max discount amount should be null or 0 for non-percentage discounts.");
        RuleFor(x => x.MinOrderAmount)
            .GreaterThan(0).WithMessage("Minimum order amount must be greater than 0.");
    }
}