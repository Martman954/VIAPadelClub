namespace VIAPadelClub.Core.Tools.ObjectMapper;

public class ObjectMapper : IObjectMapper
{
    private readonly IEnumerable<IMapping> _mappings;

    public ObjectMapper(IEnumerable<IMapping> mappings)
    {
        _mappings = mappings;
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        // 1) Use a specific mapping if one is registered
        var mapping = _mappings.OfType<IMapping<TSource, TDestination>>().FirstOrDefault();
        if (mapping is not null)
            return mapping.Convert(source);

        // 2) Otherwise fall back to default mapping
        return DefaultMap<TSource, TDestination>(source);
    }

    private static TDestination DefaultMap<TSource, TDestination>(TSource source)
    {
        var destinationType = typeof(TDestination);
        var sourceProps = typeof(TSource).GetProperties();

        // Prefer parameterless construction + property assignment when possible.
        var parameterlessCtor = destinationType.GetConstructor(Type.EmptyTypes);
        if (parameterlessCtor is not null)
        {
            var destination = Activator.CreateInstance<TDestination>();

            foreach (var destProp in destinationType.GetProperties())
            {
                if (!destProp.CanWrite) continue;

                var sourceProp = sourceProps.FirstOrDefault(p =>
                    p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase)
                    && destProp.PropertyType.IsAssignableFrom(p.PropertyType));

                if (sourceProp is not null)
                    destProp.SetValue(destination, sourceProp.GetValue(source));
            }

            return destination;
        }

        // Fallback for records/immutable types: bind constructor args by property name.
        var constructor = destinationType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor is null)
            throw new InvalidOperationException($"No usable constructor found for destination type '{destinationType.Name}'.");

        var args = constructor.GetParameters()
            .Select(param =>
            {
                var sourceProp = sourceProps.FirstOrDefault(p =>
                    p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)
                    && param.ParameterType.IsAssignableFrom(p.PropertyType));

                if (sourceProp is not null)
                    return sourceProp.GetValue(source);

                if (param.HasDefaultValue)
                    return param.DefaultValue;

                throw new InvalidOperationException(
                    $"Cannot map constructor parameter '{param.Name}' for destination type '{destinationType.Name}'.");
            })
            .ToArray();

        return (TDestination)constructor.Invoke(args);
    }
}