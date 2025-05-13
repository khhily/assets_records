using AssetsRecords.Data.Entities;
using AssetsRecords.DB;
using AssetsRecords.DB.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AssetsRecords.Repository.Assets;

public class AssetsRepository : Repository<Asset>, IAssetsRepository
{
    public AssetsRepository(AssetsRecordsDbContext context, IUnitOfWork unitOfWork) 
        : base(context, unitOfWork)
    {
    }

    /// <summary>
    /// Gets all assets for a specific batch ID
    /// </summary>
    /// <param name="batchId">The batch ID to filter by</param>
    /// <returns>A list of assets belonging to the specified batch</returns>
    public async Task<List<Asset>> GetAssetsByBatchIdAsync(int batchId)
    {
        return await _dbSet
            .Where(a => a.BatchId == batchId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Gets all assets for a list of batch IDs
    /// </summary>
    /// <param name="batchIds">The list of batch IDs to filter by</param>
    /// <returns>A list of assets belonging to any of the specified batches</returns>
    public async Task<List<Asset>> GetAssetsByBatchIdsAsync(List<int> batchIds)
    {
        return await _dbSet
            .Where(a => batchIds.Contains(a.BatchId))
            .ToListAsync();
    }
}
