using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsRecords.Repository.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all repository classes in the AssetsRecords.Repository assembly.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register the generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Get all types from the repository assembly
        var assembly = Assembly.GetExecutingAssembly();
        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository") && t != typeof(Repository<>));
        
        foreach (var repositoryType in repositoryTypes)
        {
            // Find the interface that this repository implements
            // We're looking for an interface that ends with "Repository" and is implemented by this class
            var repositoryInterface = repositoryType.GetInterfaces()
                .FirstOrDefault(i => i.Name.EndsWith("Repository") && i != typeof(IRepository<>));
            
            if (repositoryInterface != null)
            {
                // Register the repository with its interface
                services.AddScoped(repositoryInterface, repositoryType);
            }
            else
            {
                // If no specific interface is found, register the class itself
                services.AddScoped(repositoryType);
            }
        }
        
        return services;
    }
}