using FluentValidation;

namespace TheGourmet.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item.");
    }
}