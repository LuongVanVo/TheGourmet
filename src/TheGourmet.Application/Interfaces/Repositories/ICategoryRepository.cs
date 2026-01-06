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
    
    // get category by id
    Task<Category?> GetCategoryByIdAsync(Guid id);
    
    // update category in DB
    Task UpdateCategoryAsync(Category category);
    
    // delete category in DB
    Task SoftDeleteCategoryAsync(Category category);
    
    // No tracking query for categories
    IQueryable<Category> GetAllNoTrackingAsync();
}