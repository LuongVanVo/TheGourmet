using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.Features.Products.Results;

namespace TheGourmet.Application.Features.Products.Queries.GetProductsWithPagination;

public class GetProductsWithPaginationQuery : IRequest<PaginatedList<GetProductsWithPaginationResponse>>
{
    public Guid? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; } // Ví dụ: "price_asc", "price_desc", "name_asc", "name_desc"
    
    public int PageNumber { get; set; } = 1; // Mặc định trang đầu tiên
    public int PageSize { get; set; } = 10; // Mặc định 10 sản phẩm mỗi trang
}