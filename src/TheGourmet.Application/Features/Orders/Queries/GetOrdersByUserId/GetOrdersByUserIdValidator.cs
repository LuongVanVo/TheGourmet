using FluentValidation;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdValidator : AbstractValidator<GetOrdersByUserIdQuery>
{
    public GetOrdersByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}