using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly TheGourmetDbContext _dbContext;
    public VoucherRepository(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Voucher?> GetByCodeAsync(string code)
    {
        return await _dbContext.Vouchers
            .FirstOrDefaultAsync(v => v.Code == code && v.IsActive);
    }

    public async Task<Voucher?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Vouchers
            .FindAsync(id);
    }

    public async Task DecreaseQuantityAsync(Guid voucherId)
    {
        await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""Vouchers""
            SET ""Quantity"" = ""Quantity"" - 1
            WHERE ""Id"" = {voucherId} AND ""Quantity"" > 0");
    }

    public async Task AddAsync(Voucher voucher)
    {
        await _dbContext.Vouchers.AddAsync(voucher);
    }
    
    public async Task UpdateAsync(Voucher voucher)
    {
        _dbContext.Vouchers.Update(voucher);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<(List<Voucher> Items, int TotalCount)> GetVouchersWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm, bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Vouchers.AsQueryable();
        
        // Filter by SearchTerm
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(v => v.Code.ToLower().Contains(term));
        }
        
        // Filter by status (active/inactive)
        if (isActive.HasValue)
        {
            query = query.Where(v => v.IsActive == isActive.Value);
        }

        query = query.OrderByDescending(v => v.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }
}