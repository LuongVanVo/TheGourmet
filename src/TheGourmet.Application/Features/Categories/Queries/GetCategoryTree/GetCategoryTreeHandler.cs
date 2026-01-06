using MediatR;
using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.DTOs;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryTreeQuery, List<CategoryTreeDto>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<List<CategoryTreeDto>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        // call repository to get data flat list
        var allCategories = await _categoryRepository.GetAllNoTrackingAsync().ToListAsync(cancellationToken);
        
        // filter nodes root (parentId == null)
        var rootCategories = allCategories.Where(c => c.ParentId == null);
        
        // build tree recursively
        var result = rootCategories.Select(c => MapToTree(c, allCategories)).ToList();

        return result;
    }

    private CategoryTreeDto MapToTree(Category cat, List<Category> allCats)
    {
        return new CategoryTreeDto
        {
            Id = cat.Id,
            Name = cat.Name,
            // Recursively: Find children of the current category
            Children = allCats.Where(c => c.ParentId == cat.Id)
                .Select(child => MapToTree(child, allCats))
                .ToList()
        };
    }
}