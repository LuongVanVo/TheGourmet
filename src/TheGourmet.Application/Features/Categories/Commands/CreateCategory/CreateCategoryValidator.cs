using FluentValidation;

namespace TheGourmet.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả danh mục không được vượt quá 500 ký tự.");
    }
}