using FluentValidation;

namespace TheGourmet.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdValidator : AbstractValidator<GetPaymentByOrderIdQuery>
{
    public GetPaymentByOrderIdValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Order ID must be a valid GUID.");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("User ID must be a valid GUID.");
    }
}