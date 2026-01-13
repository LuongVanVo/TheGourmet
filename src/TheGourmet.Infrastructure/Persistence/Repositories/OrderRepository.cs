using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly TheGourmetDbContext _dbContext;
    public OrderRepository(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOrderAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, OrderStatus? status)
    {
        var query = _dbContext.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return await query
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }
    
    public async Task<Order?> GetByIdWithItemsAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
}