using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsRecords.Service.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all service classes in the AssetsRecords.Service assembly.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Register the generic service
        services.AddScoped(typeof(IService<>), typeof(Service<>));
        
        // Get all types from the service assembly
        var assembly = Assembly.GetExecutingAssembly();
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service") && t != typeof(Service<>));
        
        foreach (var serviceType in serviceTypes)
        {
            // Find the interface that this service implements
            // We're looking for an interface that ends with "Service" and is implemented by this class
            var serviceInterface = serviceType.GetInterfaces()
                .FirstOrDefault(i => i.Name.EndsWith("Service") && i != typeof(IService<>));
            
            if (serviceInterface != null)
            {
                // Register the service with its interface
                services.AddScoped(serviceInterface, serviceType);
            }
            else
            {
                // If no specific interface is found, register the class itself
                services.AddScoped(serviceType);
            }
        }
        
        return services;
    }
}