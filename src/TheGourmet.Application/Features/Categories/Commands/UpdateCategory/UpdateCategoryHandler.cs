using AutoMapper;
using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Categories.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // find category by id
        var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException("Category not found");
        }
        
        // map updated fields
        mapper.Map(request, category);
        
        // set audit fields
        category.LastModified = DateTime.UtcNow;
        
        // save changes
        await _categoryRepository.UpdateCategoryAsync(category);
        return new UpdateCategoryResponse
        {
            Success = true,
            Message = "Category updated successfully"
        };
    }
}