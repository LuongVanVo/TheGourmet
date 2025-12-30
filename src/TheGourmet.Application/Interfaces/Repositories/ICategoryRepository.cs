using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    // add category in DB
    Task<bool> AddCategoryAsync(Category category);
    
    // get name of category by name request
    Task<Category?> GetCategoryByNameAsync(string name);
    
    // get all categories
    Task<List<Category>> GetAllCategoriesAsync();
}