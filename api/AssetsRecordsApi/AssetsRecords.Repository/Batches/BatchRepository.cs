using AssetsRecords.Data.Entities;
using AssetsRecords.DB;
using AssetsRecords.DB.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AssetsRecords.Repository.Batches;

public class BatchRepository : Repository<AssetBatch>, IBatchRepository
{
    public BatchRepository(AssetsRecordsDbContext context, IUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    /// <summary>
    /// Checks if a batch with the specified batch number exists
    /// </summary>
    /// <param name="batchNo">The batch number to check</param>
    /// <returns>True if a batch with the specified number exists, false otherwise</returns>
    public async Task<bool> BatchNoExistsAsync(string batchNo)
    {
        return await _dbSet.AnyAsync(b => b.BatchNo == batchNo);
    }
    
    /// <summary>
    /// Gets all batches with batch numbers starting with the specified prefix
    /// </summary>
    /// <param name="batchNoPrefix">The batch number prefix to search for</param>
    /// <returns>A list of batches with matching batch number prefixes</returns>
    public async Task<List<AssetBatch>> GetBatchesByBatchNoPrefixAsync(string batchNoPrefix)
    {
        return await _dbSet
            .Where(b => b.BatchNo.StartsWith(batchNoPrefix))
            .ToListAsync();
    }
}