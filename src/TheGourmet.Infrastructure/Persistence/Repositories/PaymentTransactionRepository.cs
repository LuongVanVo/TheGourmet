using Microsoft.EntityFrameworkCore;
using TheGourmet.Application.Interfaces.Repositories;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly TheGourmetDbContext _dbContext;
    public PaymentTransactionRepository(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(pt => pt.Id == id, cancellationToken);
    }

    public async Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.PaymentTransactions.AddAsync(transaction, cancellationToken);
    }

    public void Update(PaymentTransaction transaction)
    {
        _dbContext.PaymentTransactions.Update(transaction);
    }

    public async Task<IEnumerable<PaymentTransaction>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .AsNoTracking()
            .Where(pt => pt.OrderId == orderId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentTransaction?> GetByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(pt => pt.ProviderTransactionId == providerTransactionId, cancellationToken);
    }

    public Task<IEnumerable<PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dbContext.PaymentTransactions.AsNoTracking().AsEnumerable());
    }
}