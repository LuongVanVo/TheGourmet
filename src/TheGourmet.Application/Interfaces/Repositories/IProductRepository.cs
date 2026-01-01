using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IProductRepository
{
    // add product in DB
    Task<bool> AddProductAsync(Product product);
    
    // init query 
    IQueryable<Product> GetProductsQuery(string? searchTerm, Guid? categoryId);
    
    // get product by id
    Task<Product?> GetProductByIdAsync(Guid id);
    // get product by id for admin
    Task<Product?> GetProductByIdForAdminAsync(Guid id);
    
    // update product in DB
    Task UpdateProductAsync(Product product);
    
    // delete product in DB
    Task DeleteProductAsync(Product product);
}