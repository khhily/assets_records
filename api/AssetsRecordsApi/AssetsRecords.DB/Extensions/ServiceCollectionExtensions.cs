using AssetsRecords.DB.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsRecords.DB.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AssetsRecordsDbContext to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddAssetsRecordsDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        
        services.AddDbContext<AssetsRecordsDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            
        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            
        return services;
    }
}