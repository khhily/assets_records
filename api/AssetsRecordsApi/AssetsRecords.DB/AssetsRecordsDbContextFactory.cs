using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AssetsRecords.DB;

public class AssetsRecordsDbContextFactory : IDesignTimeDbContextFactory<AssetsRecordsDbContext>
{
    public AssetsRecordsDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<AssetsRecordsDbContext>();
        optionsBuilder.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString));

        return new AssetsRecordsDbContext(optionsBuilder.Options);
    }
}