using FluentValidation;

namespace TheGourmet.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Category ID must be a valid GUID.");
    }
}