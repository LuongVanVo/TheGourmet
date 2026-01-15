using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class ProductReviewRepository : IProductReviewRepository
{
    private readonly TheGourmetDbContext _dbContext;
    public ProductReviewRepository(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ProductReview review)
    {
        await _dbContext.ProductReviews.AddAsync(review);
    }

    public async Task<bool> HasUserReviewedOrderedProductAsync(Guid userId, Guid productId, Guid orderId)
    {
        return await _dbContext.ProductReviews
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId && r.OrderId == orderId);
    }

    public async Task<List<ProductReview>> GetByProductIdAsync(Guid productId)
    {
        return await _dbContext.ProductReviews
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }

    public IQueryable<ProductReview> GetQueryable()
    {
        return _dbContext.ProductReviews.AsQueryable();
    }
}