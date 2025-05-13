using AssetsRecords.Data.Entities;

namespace AssetsRecords.Repository.Batches;

public interface IBatchRepository : IRepository<AssetBatch>
{
    /// <summary>
    /// Checks if a batch with the specified batch number exists
    /// </summary>
    /// <param name="batchNo">The batch number to check</param>
    /// <returns>True if a batch with the specified number exists, false otherwise</returns>
    Task<bool> BatchNoExistsAsync(string batchNo);
    
    /// <summary>
    /// Gets all batches with batch numbers starting with the specified prefix
    /// </summary>
    /// <param name="batchNoPrefix">The batch number prefix to search for</param>
    /// <returns>A list of batches with matching batch number prefixes</returns>
    Task<List<AssetBatch>> GetBatchesByBatchNoPrefixAsync(string batchNoPrefix);
}