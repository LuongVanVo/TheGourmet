using MediatR;
using TheGourmet.Application.Features.Orders.Results;

namespace TheGourmet.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<OrderResponse>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ReasonId { get; set; }
    public string? OtherReason { get; set; }
}