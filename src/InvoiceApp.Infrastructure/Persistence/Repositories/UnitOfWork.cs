using InvoiceApp.Domain.UnitOfWork;

namespace InvoiceApp.Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly InvoiceAppDbContext _dbContext;

    public UnitOfWork(InvoiceAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
