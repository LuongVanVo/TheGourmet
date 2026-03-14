using MediatR;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersWithPagination;

public class GetOrdersWithPaginationQuery : IRequest<PaginatedList<OrderDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; } // Find by customer name or email or number phone
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}