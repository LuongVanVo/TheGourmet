using Microsoft.EntityFrameworkCore.Storage;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TheGourmetDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;
    
    private IProductRepository? _productRepository;
    private IOrderRepository? _orderRepository;
    
    public UnitOfWork(TheGourmetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IProductRepository Products
    {
        get
        {
            return _productRepository ??= new ProductRepository(_dbContext);
        }
    }
    public IOrderRepository Orders
    {
        get
        {
            return _orderRepository ??= new OrderRepository(_dbContext);
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}