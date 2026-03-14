using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateOrderStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null)
            throw new NotFoundException("Order not found");

        // Update order status
        await _unitOfWork.Orders.UpdateOrderStatusAsync(request.OrderId, request.NewStatus);

        return new OrderResponse
        {
            Id = order.Id,
            Success = true,
            Message = "Order status updated successfully",
        };
    }
}