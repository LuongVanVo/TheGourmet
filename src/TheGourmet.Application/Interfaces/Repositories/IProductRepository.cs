using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IProductRepository
{
    // add product in DB
    Task<bool> AddProductAsync(Product product);
}