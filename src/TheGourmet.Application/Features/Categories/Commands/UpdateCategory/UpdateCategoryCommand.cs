using MediatR;
using TheGourmet.Application.Features.Categories.Results;

namespace TheGourmet.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<UpdateCategoryResponse>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}