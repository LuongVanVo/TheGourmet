using MediatR;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Features.Orders.Results;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public CancelOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
            {
                throw new NotFoundException("Order not exist.");
            }
            
            if (order.Status == OrderStatus.Cancelled)
            {
                throw new BadRequestException("Order is already cancelled.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                throw new BadRequestException("Not able to cancel this order at its current status.");
            }
            
            // Check permission
            if (order.UserId != request.UserId) 
                throw new ForbiddenException("You do not have permission to cancel this order.");
            
            // update order status
            order.Status = OrderStatus.Cancelled;
            order.ReasonCancel = request.Reason;
            
            // restock products
            foreach (var item in order.OrderItems)
            {
                await _unitOfWork.Products.IncreaseStockAtomicAsync(item.ProductId, item.Quantity);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            return new OrderResponse
            {
                Id = request.OrderId,
                Success = true,
                Message = "Order cancelled successfully."
            };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}