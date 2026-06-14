using Microsoft.Extensions.DependencyInjection;

namespace VIAPadelClub.Core.QueryContracts;

public static class QueryDependencyInjectionExtensions
{
    /// <summary>
    /// Registers query dispatcher and optionally scans provided assemblies for query handlers.
    /// </summary>
    public static IServiceCollection AddApplicationQueryDispatch(
        this IServiceCollection services,
        params System.Reflection.Assembly[] queryHandlerAssemblies)
    {
        foreach (var assembly in queryHandlerAssemblies.Distinct())
        {
            var handlerRegistrations = assembly
                .GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false })
                .SelectMany(implementation => implementation
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                    .Select(service => new { service, implementation }));

            foreach (var registration in handlerRegistrations)
                services.AddScoped(registration.service, registration.implementation);
        }

        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }
}

