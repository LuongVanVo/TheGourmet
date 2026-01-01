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
            .Where(p => p.IsActive == true)
            .AsNoTracking();

        // filter by category id
        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }
        
        // filter by search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Unaccent keyword and use ILike for case-insensitive search
            var normalizedKeyword = $"%{searchTerm.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(
                EF.Functions.Unaccent(x.Name), 
                EF.Functions.Unaccent(normalizedKeyword)
            ));
        }
        
        return query;
    }
    
    // get product by id
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive == true);
    }
    
    // get product by id for admin
    public async Task<Product?> GetProductByIdForAdminAsync(Guid id)
    {
        return await dbContext.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    // update product in DB
    public async Task UpdateProductAsync(Product product)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();
    }
    
    // delete product in DB
    public async Task DeleteProductAsync(Product product)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
}