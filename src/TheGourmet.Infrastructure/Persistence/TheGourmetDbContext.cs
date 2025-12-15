using Microsoft.EntityFrameworkCore;

namespace TheGourmet.Infrastructure.Persistence;

public class TheGourmetDbContext : DbContext
{
    public TheGourmetDbContext(DbContextOptions<TheGourmetDbContext> options) : base(options)
    {
        
    }
}