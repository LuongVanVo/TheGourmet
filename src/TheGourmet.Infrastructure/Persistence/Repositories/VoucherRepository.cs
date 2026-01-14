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
}