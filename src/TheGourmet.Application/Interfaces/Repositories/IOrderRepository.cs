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
}