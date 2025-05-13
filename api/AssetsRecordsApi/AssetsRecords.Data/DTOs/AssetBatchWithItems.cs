using AssetsRecords.Data.Entities;

namespace AssetsRecords.Data.DTOs;

/// <summary>
/// DTO for returning batches with their assets
/// </summary>
public class AssetBatchWithItems
{
    public AssetBatch Batch { get; set; } = new();
    public List<Asset> Assets { get; set; } = new();
}