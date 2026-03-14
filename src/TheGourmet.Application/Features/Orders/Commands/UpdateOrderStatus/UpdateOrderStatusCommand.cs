using MediatR;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<OrderResponse>
{
    public Guid OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}