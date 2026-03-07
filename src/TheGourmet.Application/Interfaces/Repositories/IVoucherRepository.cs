using TheGourmet.Domain.Entities;

namespace TheGourmet.Application.Interfaces.Repositories;

public interface IVoucherRepository
{
    Task<Voucher?> GetByCodeAsync(string code);
    Task<Voucher?> GetByIdAsync(Guid id);
    Task DecreaseQuantityAsync(Guid voucherId);
    Task AddAsync(Voucher voucher);
    Task UpdateAsync(Voucher voucher);

    Task<(List<Voucher> Items, int TotalCount)> GetVouchersWithPaginationAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        bool? isActive,
        CancellationToken cancellationToken = default
    );
}