using MediatR;
using TheGourmet.Application.Features.Orders.Results;

namespace TheGourmet.Application.Features.Orders.Commands.ConfirmReceipt;

public class ConfirmReceiptCommand : IRequest<OrderResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}