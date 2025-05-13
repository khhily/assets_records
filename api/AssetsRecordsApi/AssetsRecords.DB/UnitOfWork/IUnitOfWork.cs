namespace AssetsRecords.DB.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Begins a new transaction and returns a transaction scope.
    /// </summary>
    Task<ITransactionScope> BeginTransactionAsync();
    
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task SaveChangesAsync();
}

/// <summary>
/// Represents a transaction scope that can be committed or rolled back.
/// </summary>
public interface ITransactionScope : IAsyncDisposable
{
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    Task CommitAsync();
}
