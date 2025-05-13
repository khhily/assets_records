using AssetsRecords.Data.DTOs;
using AssetsRecords.Data.Entities;

namespace AssetsRecords.Service.Assets;

public interface IAssetsService
{
    // 1. Create assets with batch
    Task<AssetBatch> CreateAssetsAsync(List<Asset> assets);
    
    // 2. Update assets by batch ID
    Task UpdateAssetsByBatchIdAsync(int batchId, List<Asset> assets);
    
    // 3. Delete assets by batch ID
    Task DeleteAssetsByBatchIdAsync(int batchId);
    
    // 4. Get all batches with their assets
    Task<List<AssetBatchWithItems>> GetAllBatchesWithAssetsAsync();
}
