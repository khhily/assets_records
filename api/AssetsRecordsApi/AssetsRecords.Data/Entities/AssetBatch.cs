using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetsRecords.Data.Entities;

public class AssetBatch
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string BatchNo { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedTime { get; set; }
    
    [Required]
    public DateTime LastModifiedTime { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    // Navigation property
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}