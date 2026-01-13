using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
    Task<Order> GetByIdWithItemsAsync(Guid orderId);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
}