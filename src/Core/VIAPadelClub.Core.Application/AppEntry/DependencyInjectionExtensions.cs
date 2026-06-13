using Microsoft.Extensions.DependencyInjection;

namespace VIAPadelClub.Core.Application.AppEntry;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers command dispatcher and all ICommandHandler&lt;TCommand&gt; implementations from this assembly.
    /// </summary>
    public static IServiceCollection AddApplicationCommandDispatch(this IServiceCollection services)
    {
        var assembly = typeof(ICommandDispatcher).Assembly;

        var handlerRegistrations = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(implementation => implementation
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(service => new { service, implementation }));

        foreach (var registration in handlerRegistrations)
            services.AddScoped(registration.service, registration.implementation);

        services.AddScoped<ICommandDispatcher, Dispatcher>();

        return services;
    }
}

