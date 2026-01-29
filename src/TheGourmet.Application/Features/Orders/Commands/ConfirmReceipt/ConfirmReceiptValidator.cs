using FluentValidation;

namespace TheGourmet.Application.Features.Orders.Commands.ConfirmReceipt;

public class ConfirmReceiptValidator : AbstractValidator<ConfirmReceiptCommand>
{
    public ConfirmReceiptValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Order ID must be a valid GUID.");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}