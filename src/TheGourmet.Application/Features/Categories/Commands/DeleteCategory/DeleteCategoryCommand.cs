using MediatR;
using TheGourmet.Application.Features.Categories.Results;

namespace TheGourmet.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<CategoryResponse> 
{
    public Guid Id { get; set; }
}