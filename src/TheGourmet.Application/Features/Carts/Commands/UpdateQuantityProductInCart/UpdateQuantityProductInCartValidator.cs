using FluentValidation;

namespace TheGourmet.Application.Features.Carts.Commands.UpdateQuantityProductInCart;

public class UpdateQuantityProductInCartValidator : AbstractValidator<UpdateQuantityProductInCartCommand>
{
    public UpdateQuantityProductInCartValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .Must(x => x != Guid.Empty).WithMessage("UserId must be a valid GUID.");
        
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required")
            .Must(x => x != Guid.Empty).WithMessage("ProductId must be a valid GUID.");
    }
}