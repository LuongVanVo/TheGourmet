using FluentValidation;

namespace TheGourmet.Application.Features.Products.Commands.ToggleActiveProduct;

public class ToggleActiveProductValidator : AbstractValidator<ToggleActiveProductCommand>
{
    public ToggleActiveProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Product ID must be a valid GUID.");
    }
}