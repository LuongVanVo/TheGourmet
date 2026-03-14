using FluentValidation;

namespace TheGourmet.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("OrderId must be a valid GUID.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("NewStatus must be a valid OrderStatus enum value.");
    }
}