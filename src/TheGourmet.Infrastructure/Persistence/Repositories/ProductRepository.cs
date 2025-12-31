using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class ProductRepository(TheGourmetDbContext dbContext) : IProductRepository
{
    // add product in DB
    public async Task<bool> AddProductAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
    
    // get products query
    public IQueryable<Product> GetProductsQuery(string? searchTerm, Guid? categoryId)
    {
        var query = dbContext.Products
            .Include(p => p.Category)
            .AsNoTracking();

        // filter by category id
        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }
        
        // filter by search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            query = query.Where(x => EF.Functions.ILike(
                EF.Functions.Unaccent(x.Name), 
                EF.Functions.Unaccent($"%{keyword}%")
            ));
        }
        
        return query;
    }
}