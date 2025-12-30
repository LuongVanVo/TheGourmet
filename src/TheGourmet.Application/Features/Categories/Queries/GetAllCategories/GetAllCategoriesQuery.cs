using MediatR;
using TheGourmet.Application.Features.Categories.Results;

namespace TheGourmet.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>
{
    
}