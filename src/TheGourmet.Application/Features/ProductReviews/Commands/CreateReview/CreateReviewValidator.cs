using FluentValidation;

namespace TheGourmet.Application.Features.ProductReviews.Commands.CreateReview;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("UserId must be a valid GUID");
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("ProductId must be a valid GUID");
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId should not be empty")
            .Must(x => x != Guid.Empty).WithMessage("OrderId must be a valid GUID");
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}