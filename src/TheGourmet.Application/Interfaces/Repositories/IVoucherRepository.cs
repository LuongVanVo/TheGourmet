using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IVoucherRepository
{
    Task<Voucher?> GetByCodeAsync(string code);
    Task<Voucher?> GetByIdAsync(Guid id);
    Task DecreaseQuantityAsync(Guid voucherId);
    Task AddAsync(Voucher voucher);
}