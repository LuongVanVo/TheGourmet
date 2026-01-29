using Hangfire;
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

            if (order.Status == OrderStatus.Shipping)
            {
                throw new BadRequestException("Order is already being shipped and cannot be cancelled.");
            }
            
            if (order.Status != OrderStatus.Pending)
            {
                throw new BadRequestException("Not able to cancel this order at its current status.");
            }

            
            // Check permission
            if (order.UserId != request.UserId) 
                throw new ForbiddenException("You do not have permission to cancel this order.");
            
            // Remove job check paid of Hangfire
            if (!string.IsNullOrEmpty(order.HangfireJobId))
            {
                BackgroundJob.Delete(order.HangfireJobId);
                order.HangfireJobId = null;
            }
            
            // update order status
            order.Status = OrderStatus.Cancelled;
            string reason = "Không có lý do cụ thể";

            if (request.ReasonId.HasValue)
            {
                var reasonTemplate = await _unitOfWork.OrderCancelReasons.GetByIdAsync(request.ReasonId.Value);
                if (reasonTemplate != null)
                {
                    reason = reasonTemplate.Description;
                }
            } 
            else if (!string.IsNullOrWhiteSpace(request.OtherReason))
            {
                reason = request.OtherReason;
            }
            order.ReasonCancel = reason;
            order.CanceledDate = DateTime.UtcNow;
            
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