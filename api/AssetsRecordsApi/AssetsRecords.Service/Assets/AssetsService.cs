using AssetsRecords.Data.DTOs;
using AssetsRecords.Data.Entities;
using AssetsRecords.DB.UnitOfWork;
using AssetsRecords.Repository.Assets;
using AssetsRecords.Repository.Batches;

namespace AssetsRecords.Service.Assets;

public class AssetsService : IAssetsService
{
    private readonly IBatchRepository _batchRepository;
    private readonly IAssetsRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssetsService(
        IBatchRepository batchRepository,
        IAssetsRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _batchRepository = batchRepository;
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new asset batch with associated assets
    /// </summary>
    public async Task<AssetBatch> CreateAssetsAsync(List<Asset> assets)
    {
        // Begin a transaction that will automatically roll back if not committed
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        // Generate a unique batch number
        string batchNo = await GenerateUniqueBatchNumberAsync();
        
        // Create a new batch
        var batch = new AssetBatch
        {
            BatchNo = batchNo,
            CreatedTime = DateTime.Now,
            LastModifiedTime = DateTime.Now,
            // Calculate total amount from assets
            TotalAmount = assets.Sum(a => a.Amount)
        };
        
        // Save batch first to get ID
        await _batchRepository.AddAsync(batch);
        await _unitOfWork.SaveChangesAsync();
        
        // Assign batch ID to assets and save them
        foreach (var asset in assets)
        {
            asset.BatchId = batch.Id;
            await _assetRepository.AddAsync(asset);
        }
        
        // Commit the transaction
        await transaction.CommitAsync();
        
        return batch;
    }

    /// <summary>
    /// Generates a unique batch number based on the current date
    /// </summary>
    /// <returns>A unique batch number</returns>
    private async Task<string> GenerateUniqueBatchNumberAsync()
    {
        // Generate base batch number using current date (yyyyMMdd)
        string baseBatchNo = DateTime.Now.ToString("yyyyMMdd");
        
        // Get all existing batches with the same base batch number
        var existingBatches = await _batchRepository.GetBatchesByBatchNoPrefixAsync(baseBatchNo);
        
        // Determine the batch number
        string batchNo;
        if (existingBatches.Count == 0)
        {
            // No existing batches with this date, use the base batch number
            batchNo = baseBatchNo;
        }
        else
        {
            // Find the highest suffix number and increment it
            int maxSuffix = 0;
            foreach (var existingBatch in existingBatches)
            {
                // Check if the batch number has a suffix
                if (existingBatch.BatchNo.Length > baseBatchNo.Length && 
                    existingBatch.BatchNo[baseBatchNo.Length] == '-')
                {
                    // Try to parse the suffix
                    string suffixStr = existingBatch.BatchNo.Substring(baseBatchNo.Length + 1);
                    if (int.TryParse(suffixStr, out int suffix) && suffix > maxSuffix)
                    {
                        maxSuffix = suffix;
                    }
                }
            }
            
            // Create new batch number with incremented suffix
            batchNo = $"{baseBatchNo}-{(maxSuffix + 1):00}";
        }
        
        // Double-check that the batch number is unique
        if (await _batchRepository.BatchNoExistsAsync(batchNo))
        {
            // In the unlikely event of a collision, recursively try again with a higher suffix
            return await GenerateUniqueBatchNumberAsync();
        }
        
        return batchNo;
    }

    /// <summary>
    /// Updates assets for a specific batch ID
    /// </summary>
    public async Task UpdateAssetsByBatchIdAsync(int batchId, List<Asset> updatedAssets)
    {
        // Begin a transaction that will automatically roll back if not committed
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        // Get the batch
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null)
        {
            throw new KeyNotFoundException($"Batch with ID {batchId} not found");
        }
        
        // Update batch timestamp
        batch.LastModifiedTime = DateTime.Now;
        
        // Get existing assets for this batch
        var existingAssets = await _assetRepository.GetAssetsByBatchIdAsync(batchId);
        var existingIds = existingAssets.Select(a => a.Id).ToList();
        
        // Process updated assets
        foreach (var asset in updatedAssets)
        {
            asset.BatchId = batchId; // Ensure correct batch ID
            
            if (asset.Id == 0)
            {
                // New asset
                await _assetRepository.AddAsync(asset);
            }
            else
            {
                var exists = existingAssets.Single(q => q.Id == asset.Id);
                exists.Amount = asset.Amount;
                exists.Name = asset.Name;
                exists.AssetType = asset.AssetType;
                exists.MaturityDate = asset.MaturityDate;
                // Existing asset to update
                await _assetRepository.UpdateAsync(exists);
                existingIds.Remove(asset.Id);
            }
        }
        
        // Delete assets that are no longer in the list
        foreach (var idToDelete in existingIds)
        {
            var assetToDelete = await _assetRepository.GetByIdAsync(idToDelete);
            if (assetToDelete != null)
            {
                await _assetRepository.DeleteAsync(assetToDelete);
            }
        }
        
        // Update batch total amount
        batch.TotalAmount = updatedAssets.Sum(a => a.Amount);
        await _batchRepository.UpdateAsync(batch);
        
        // Commit the transaction
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Deletes a batch and all its associated assets
    /// </summary>
    public async Task DeleteAssetsByBatchIdAsync(int batchId)
    {
        // Begin a transaction that will automatically roll back if not committed
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        
        // Get the batch
        var batch = await _batchRepository.GetByIdAsync(batchId);
        if (batch == null)
        {
            throw new KeyNotFoundException($"Batch with ID {batchId} not found");
        }
        
        // Get all assets for this batch
        var assets = await _assetRepository.GetAssetsByBatchIdAsync(batchId);
        
        // Delete all assets first
        foreach (var asset in assets)
        {
            await _assetRepository.DeleteAsync(asset);
        }
        
        // Then delete the batch
        await _batchRepository.DeleteAsync(batch);
        
        // Commit the transaction
        await transaction.CommitAsync();
    }

    /// <summary>
    /// Gets all batches with their associated assets
    /// </summary>
    public async Task<List<AssetBatchWithItems>> GetAllBatchesWithAssetsAsync()
    {
        // Get all batches
        var batches = (await _batchRepository.GetAllAsync()).ToList();
        
        // Get all batch IDs
        var batchIds = batches.Select(b => b.Id).ToList();
        
        // Get all assets for these batches in a single query
        var allAssets = await _assetRepository.GetAssetsByBatchIdsAsync(batchIds);
        
        // Group assets by batch ID
        var assetsByBatchId = allAssets.GroupBy(a => a.BatchId)
                                       .ToDictionary(g => g.Key, g => g.ToList());
        
        // Create result list
        var result = new List<AssetBatchWithItems>();
        
        // Assemble the result
        foreach (var batch in batches)
        {
            result.Add(new AssetBatchWithItems
            {
                Batch = batch,
                Assets = assetsByBatchId.TryGetValue(batch.Id, out var assets) ? assets : new List<Asset>()
            });
        }
        
        return result;
    }
}
