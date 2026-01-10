using MediatR;
using TheGourmet.Application.DTOs.Order;
using TheGourmet.Application.Features.Orders.Results;

namespace TheGourmet.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public Guid UserId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
}