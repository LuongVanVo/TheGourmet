using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class CategoryRepository(TheGourmetDbContext dbContext) : ICategoryRepository
{
    private readonly TheGourmetDbContext _dbContext = dbContext;
    
    public async Task<bool> AddCategoryAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
    
    // get name of category by name
    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
    }
    
    // get all categories
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _dbContext.Categories.ToListAsync();
    }
    
    // get category by id
    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await _dbContext.Categories.FindAsync(id);
    }
}