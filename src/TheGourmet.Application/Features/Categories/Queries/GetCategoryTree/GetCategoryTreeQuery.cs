using MediatR;
using TheGourmet.Application.DTOs.Category;

namespace TheGourmet.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeQuery : IRequest<List<CategoryTreeDto>>
{
    
}