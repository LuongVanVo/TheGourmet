using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Categories.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category with the same name exists
        var existingCategory = await _categoryRepository.GetCategoryByNameAsync(request.Name);
        if (existingCategory != null)
        {
            throw new BadRequestException("Category with the same name already exists");
        }
        // Map data from Command to Category entity
        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
        };
        
        // Add to db
        var result = await _categoryRepository.AddCategoryAsync(entity);

        if (!result)
        {
            throw new BadRequestException($"Failed to create category {request.Name}");
        }

        return new CategoryResponse
        {
            Success = true,
            Message = "Category created successfully",
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            ImageUrl = entity.ImageUrl ?? string.Empty
        };
    }
}