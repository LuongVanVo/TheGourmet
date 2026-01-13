using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Common.Models;
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
    public IQueryable<Product> GetProductsQuery(ProductQueryParameters parameters)
    {
        var query = dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive == true)
            .AsNoTracking();

        // filter by category id
        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == parameters.CategoryId.Value);
        }
        
        // filter by search term
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            // Unaccent keyword and use ILike for case-insensitive search
            var normalizedKeyword = $"%{parameters.SearchTerm.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(
                EF.Functions.Unaccent(x.Name), 
                EF.Functions.Unaccent(normalizedKeyword)
            ));
        }
        
        // Filter by price range
        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= parameters.MaxPrice.Value);
        }
        
        // Sorting
        var sortOption = string.IsNullOrWhiteSpace(parameters.Sort) ? "newest" : parameters.Sort.ToLower();

        switch (sortOption)
        {
            case "price_asc":
                query = query.OrderBy(x => x.Price);
                break;
            case "price_desc":
                query = query.OrderByDescending(x => x.Price);
                break;
            case "oldest":
                query = query.OrderBy(x => x.Created);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
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
    
    // transaction for product
    public async Task<bool> DecreaseStockAtomicAsync(Guid productId, int quantity)
    {
        int rowsAffected = await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""Products"" 
            SET ""StockQuantity"" = ""StockQuantity"" - {quantity}
            WHERE ""Id"" = {productId} AND ""StockQuantity"" >= {quantity}");

        return rowsAffected > 0;
    }

    public async Task IncreaseStockAtomicAsync(Guid productId, int quantity)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
        UPDATE ""Products"" 
        SET ""StockQuantity"" = ""StockQuantity"" + {quantity}
        WHERE ""Id"" = {productId}");
    }
}