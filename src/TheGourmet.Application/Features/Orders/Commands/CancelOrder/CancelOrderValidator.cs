using FluentValidation;

namespace TheGourmet.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.")
            .Must(x => x != Guid.Empty).WithMessage("Order ID must be a valid GUID.");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(x => x != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}