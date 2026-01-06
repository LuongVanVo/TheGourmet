using FluentValidation;

namespace TheGourmet.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Category ID must be a valid GUID.");
    }
}