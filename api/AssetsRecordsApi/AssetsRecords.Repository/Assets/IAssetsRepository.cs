using AssetsRecords.Data.Entities;

namespace AssetsRecords.Repository.Assets;

public interface IAssetsRepository : IRepository<Asset>
{
    /// <summary>
    /// Gets all assets for a specific batch ID
    /// </summary>
    /// <param name="batchId">The batch ID to filter by</param>
    /// <returns>A list of assets belonging to the specified batch</returns>
    Task<List<Asset>> GetAssetsByBatchIdAsync(int batchId);
    
    /// <summary>
    /// Gets all assets for a list of batch IDs
    /// </summary>
    /// <param name="batchIds">The list of batch IDs to filter by</param>
    /// <returns>A list of assets belonging to any of the specified batches</returns>
    Task<List<Asset>> GetAssetsByBatchIdsAsync(List<int> batchIds);
}
