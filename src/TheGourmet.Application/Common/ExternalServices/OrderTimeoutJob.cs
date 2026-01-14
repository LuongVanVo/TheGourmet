using Microsoft.Extensions.Logging;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Common.ExternalServices;

public class OrderTimeoutJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderTimeoutJob> _logger;
    public OrderTimeoutJob(IUnitOfWork unitOfWork, ILogger<OrderTimeoutJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task CheckAndCancelOrderAsync(Guid orderId)
    {
        _logger.LogInformation($"[Hangfire] Start checking status for order {orderId}");
        
        // get order by id
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning($"[Hangfire] Order {orderId} not found.");
            return;
        }
        
        // Only cancel when order is still pending, if user has paid the order, do nothing
        if (order.Status == OrderStatus.Pending)
        {
            try
            {
                order.Status = OrderStatus.Cancelled;
                order.ReasonCancel = "System: Tự động hủy do quá hạn thanh toán (24h).";
                
                // restock products
                foreach (var item in order.OrderItems)
                {
                    await _unitOfWork.Products.IncreaseStockAtomicAsync(item.ProductId, item.Quantity);
                }
                
                // save db
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync(); // if functon IncreaseStockAtomicAsync not commit, commit here
                
                _logger.LogInformation($"[Hangfire] Order {orderId} has been cancelled due to payment timeout.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Hangfire] Error cancelling order {orderId}");
                throw;
            }
        }
        else
        {
            _logger.LogInformation($"[Hangfire] Order {orderId} is already in status {order.Status}, no action taken.");
        }
    }
}