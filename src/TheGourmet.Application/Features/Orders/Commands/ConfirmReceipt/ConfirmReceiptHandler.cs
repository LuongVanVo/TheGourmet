using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.ConfirmReceipt;

public class ConfirmReceiptHandler : IRequestHandler<ConfirmReceiptCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public ConfirmReceiptHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> Handle(ConfirmReceiptCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id);
        if (order == null)
        {
            throw new NotFoundException("Order not exist.");
        }
        
        // Check permission
        if (order.UserId != request.UserId)
        {
            throw new ForbiddenException("You do not have permission to confirm receipt of this order.");
        }

        if (order.Status != OrderStatus.Shipping)
        {
            throw new BadRequestException("Only orders in 'Shipping' status can be confirmed for receipt.");
        }

        order.Status = OrderStatus.Completed;
        order.CompletedDate = DateTime.UtcNow;

        await _unitOfWork.Orders.UpdateOrderAsync(order);

        return new OrderResponse
        {
            Id = order.Id,
            Success = true,
            Message = "Order receipt confirmed successfully."
        };
    }
}