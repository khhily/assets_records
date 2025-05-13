using AssetsRecords.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetsRecords.DB;

public class AssetsRecordsDbContext : DbContext
{
    public AssetsRecordsDbContext(DbContextOptions<AssetsRecordsDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetBatch> AssetBatches { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Batch)
            .WithMany(b => b.Assets)
            .HasForeignKey(a => a.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure indexes
        modelBuilder.Entity<AssetBatch>()
            .HasIndex(b => b.BatchNo)
            .IsUnique();
    }
}