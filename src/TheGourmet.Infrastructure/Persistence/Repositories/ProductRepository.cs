using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly TheGourmetDbContext _DbContext;
    public ProductRepository(TheGourmetDbContext dbContext)
    {
        _DbContext = dbContext;
    }
    // add product in DB
    public async Task<bool> AddProductAsync(Product product)
    {
        await _DbContext.Products.AddAsync(product);
        var result = await _DbContext.SaveChangesAsync();
        return result > 0;
    }
}