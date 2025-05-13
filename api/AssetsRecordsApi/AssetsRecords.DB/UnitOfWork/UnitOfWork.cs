using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AssetsRecords.DB.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AssetsRecordsDbContext _dbContext;
    private bool _disposed;

    public UnitOfWork(AssetsRecordsDbContext dbContext)
    {
        _dbContext = dbContext;
        _disposed = false;
    }

    /// <summary>
    /// Begins a new transaction and returns a transaction scope.
    /// </summary>
    public async Task<ITransactionScope> BeginTransactionAsync()
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        return new TransactionScope(_dbContext, transaction);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Disposes the resources used by the unit of work.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            // We don't dispose the DbContext here as it's managed by the DI container
            _disposed = true;
        }
    }

    /// <summary>
    /// Implementation of ITransactionScope that manages a database transaction.
    /// </summary>
    private class TransactionScope : ITransactionScope
    {
        private readonly AssetsRecordsDbContext _dbContext;
        private readonly IDbContextTransaction _transaction;
        private bool _committed;
        private bool _disposed;

        public TransactionScope(AssetsRecordsDbContext dbContext, IDbContextTransaction transaction)
        {
            _dbContext = dbContext;
            _transaction = transaction;
            _committed = false;
            _disposed = false;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public async Task CommitAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(TransactionScope));
            }

            await _dbContext.SaveChangesAsync();
            await _transaction.CommitAsync();
            _committed = true;
        }

        /// <summary>
        /// Disposes the transaction, rolling it back if not committed.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!_disposed)
            {
                if (!_committed)
                {
                    // If the transaction was not committed, roll it back
                    await _transaction.RollbackAsync();
                }

                await _transaction.DisposeAsync();
                _disposed = true;
            }
        }
    }
}