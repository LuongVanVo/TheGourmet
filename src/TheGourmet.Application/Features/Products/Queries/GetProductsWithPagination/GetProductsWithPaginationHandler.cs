using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Products.Queries.GetProductsWithPagination;

public class GetProductsWithPaginationHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsWithPaginationQuery, PaginatedList<GetProductsWithPaginationResponse>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public Task<PaginatedList<GetProductsWithPaginationResponse>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var parameters = new ProductQueryParameters
        {
            SearchTerm = request.SearchTerm,
            CategoryId = request.CategoryId,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Sort = request.Sort,
        };
        
        var query = _productRepository.GetProductsQuery(parameters);
        
        // map to response
        var mappedQuery = query.Select(x => new GetProductsWithPaginationResponse
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            OriginalPrice = x.OriginalPrice,
            StockQuantity = x.StockQuantity,
            ImageUrl = x.ImageUrl,
            CategoryId = x.CategoryId,
            CategoryName = x.Category.Name
        });
        
        // return paginated list
        return PaginatedList<GetProductsWithPaginationResponse>.CreateAsync(mappedQuery, request.PageNumber,
            request.PageSize);
    }
}