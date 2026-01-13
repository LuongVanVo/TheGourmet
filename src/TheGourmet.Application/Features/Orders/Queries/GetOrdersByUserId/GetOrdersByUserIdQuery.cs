using MediatR;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdQuery : IRequest<List<OrderDto>>
{
    public Guid UserId { get; set; }
    public OrderStatus? Status { get; set; }
}