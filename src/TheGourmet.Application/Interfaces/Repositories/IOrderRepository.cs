using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
}