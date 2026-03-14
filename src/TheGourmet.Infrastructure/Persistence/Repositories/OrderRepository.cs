using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Common.Models;
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
    
    // Get order by id
    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
    
    // Update order
    public Task UpdateOrderAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        return _dbContext.SaveChangesAsync();
    }

    // Get orders with pagination
    public async Task<PaginatedList<Order>> GetOrdersWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm, OrderStatus? status,
        DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // Filter by search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(o =>
                o.ReceiverName.ToLower().Contains(search) ||
                o.ReceiverPhone.ToLower().Contains(search)
            );
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedDate <= toDate.Value);
        }

        // Count total items before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination
        var items = await query
            .OrderByDescending(o => o.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Order>(items, totalCount, pageNumber, pageSize);
    }

    // Update order status
    public Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        return _dbContext.Orders
            .Where(o => o.Id == orderId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(o => o.Status, newStatus));
    }
}