using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class OrderCancelReasonRepository : IOrderCancelReasonRepository
{
    private readonly TheGourmetDbContext _dbContext;
    public OrderCancelReasonRepository(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderCancelReason?> GetByIdAsync(Guid id)
    {
        return await _dbContext.OrderCancelReasons.FindAsync(id);
    }

    public async Task<List<OrderCancelReason>> GetAllAsync()
    {
        return await _dbContext.OrderCancelReasons
            .Where(o => o.IsActive == true)
            .ToListAsync();
    }
}