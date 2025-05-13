using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AssetsRecords.Data.Enums;

namespace AssetsRecords.Data.Entities;

public class Asset
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public AssetType AssetType { get; set; }
    
    [Required]
    public int BatchId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    // Optional field
    public DateTime? MaturityDate { get; set; }
    
    // Navigation property
    [ForeignKey("BatchId")]
    public virtual AssetBatch? Batch { get; set; }
}