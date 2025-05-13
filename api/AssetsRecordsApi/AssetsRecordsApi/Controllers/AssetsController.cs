using AssetsRecords.Data.DTOs;
using AssetsRecords.Data.Entities;
using AssetsRecords.Service.Assets;
using Microsoft.AspNetCore.Mvc;

namespace AssetsRecordsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsService _assetsService;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IAssetsService assetsService, ILogger<AssetsController> logger)
    {
        _assetsService = assetsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all asset batches with their associated assets
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AssetBatchWithItems>>> GetAllAssets()
    {
        try
        {
            var result = await _assetsService.GetAllBatchesWithAssetsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets");
            return StatusCode(500, "An error occurred while retrieving assets");
        }
    }

    /// <summary>
    /// Creates a new asset batch with associated assets
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AssetBatch>> CreateAssets(CreateAssetsRequest request)
    {
        try
        {
            var result = await _assetsService.CreateAssetsAsync(request.Assets);
            return CreatedAtAction(nameof(GetAllAssets), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assets");
            return StatusCode(500, "An error occurred while creating assets");
        }
    }

    /// <summary>
    /// Updates assets for a specific batch
    /// </summary>
    [HttpPut("{batchId}")]
    public async Task<IActionResult> UpdateAssets(int batchId, [FromBody] CreateAssetsRequest request)
    {
        try
        {
            await _assetsService.UpdateAssetsByBatchIdAsync(batchId, request.Assets);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assets for batch {BatchId}", batchId);
            return StatusCode(500, "An error occurred while updating assets");
        }
    }

    /// <summary>
    /// Deletes a batch and all its associated assets
    /// </summary>
    [HttpDelete("{batchId}")]
    public async Task<IActionResult> DeleteAssets(int batchId)
    {
        try
        {
            await _assetsService.DeleteAssetsByBatchIdAsync(batchId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assets for batch {BatchId}", batchId);
            return StatusCode(500, "An error occurred while deleting assets");
        }
    }
}