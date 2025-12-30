using MediatR;
using TheGourmet.Application.Features.Categories.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IEnumerable<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync();
        if (!categories.Any())
        {
            return [];
        }
        
        return categories.Select(category => new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl ?? string.Empty,
            Success = true,
            Message = "Category retrieved successfully"
        });
    }
}