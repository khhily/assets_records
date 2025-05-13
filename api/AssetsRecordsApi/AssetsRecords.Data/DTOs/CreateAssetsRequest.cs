using AssetsRecords.Data.Entities;

namespace AssetsRecords.Data.DTOs;

/// <summary>
/// DTO for creating a new batch of assets
/// </summary>
public class CreateAssetsRequest
{
    /// <summary>
    /// List of assets to create in a new batch
    /// </summary>
    public List<Asset> Assets { get; set; } = new();
}