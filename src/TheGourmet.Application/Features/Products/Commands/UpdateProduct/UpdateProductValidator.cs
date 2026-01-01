using FluentValidation;

namespace TheGourmet.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Product ID must be a valid GUID.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        
        RuleFor(x => x.OriginalPrice)
            .GreaterThan(0).WithMessage("Original Price must be greater than zero.")
            .LessThanOrEqualTo(x => x.Price ?? decimal.MaxValue).WithMessage("Original Price must be less than or equal to Price.");
        
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock Quantity cannot be negative.");
        
        RuleFor(x => x.CategoryId)
            .Must(id => id == null || id != Guid.Empty).WithMessage("Category ID must be a valid GUID if provided.");
    }
}