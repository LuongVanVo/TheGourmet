using MediatR;
using TheGourmet.Application.DTOs.Order;

namespace TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersByUserIdQuery : IRequest<List<OrderDto>>
{
    public Guid UserId { get; set; }
}