using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.QueryContracts;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers all IQueryHandler<TQuery, TAnswer> implementations from this assembly.
    /// Query dispatcher is registered in Core.Application.
    /// </summary>
    public static IServiceCollection AddEfcQueries(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjectionExtensions).Assembly;

        var handlerRegistrations = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(implementation => implementation
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Select(service => new { service, implementation }));

        foreach (var registration in handlerRegistrations)
            services.AddScoped(registration.service, registration.implementation);

        return services;
    }
}


