using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Categories.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // find category by id
        var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException("Category not found");
        }
        
        // update category properties
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            category.Name = request.Name;
        }
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            category.Description = request.Description;
        }
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            category.ImageUrl = request.ImageUrl;
        }
        
        // save changes
        await _categoryRepository.UpdateCategoryAsync(category);
        return new UpdateCategoryResponse
        {
            Success = true,
            Message = "Category updated successfully"
        };
    }
}