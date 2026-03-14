using TheGourmet.Application.Common.Models;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
    Task<Order> GetByIdWithItemsAsync(Guid orderId);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, OrderStatus? status);
    // Get order by id
    Task<Order?> GetByIdAsync(Guid orderId);
    // Update order in DB
    Task UpdateOrderAsync(Order order);

    // Get orders with pagination
    Task<PaginatedList<Order>> GetOrdersWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm, OrderStatus? status, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken);

    // Update order status
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
}