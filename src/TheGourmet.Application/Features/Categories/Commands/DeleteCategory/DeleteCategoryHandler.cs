using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Categories.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<DeleteCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<CategoryResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // find category by id
        var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException("Category not found");
        }
        
        // delete category
        await _categoryRepository.SoftDeleteCategoryAsync(category);

        return new CategoryResponse
        {
            Success = true,
            Message = "Category deleted successfully",
            Id = category.Id,
            Description = category.Description,
            Name = category.Name,
            ImageUrl = category.ImageUrl ?? string.Empty
        };
    }
}