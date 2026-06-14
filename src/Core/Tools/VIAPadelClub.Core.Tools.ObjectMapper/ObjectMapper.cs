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

        // 2) Otherwise fall back to default name-based mapping
        return DefaultMap<TSource, TDestination>(source);
    }

    private static TDestination DefaultMap<TSource, TDestination>(TSource source)
    {
        var destination = Activator.CreateInstance<TDestination>();
        var sourceProps = typeof(TSource).GetProperties();

        foreach (var destProp in typeof(TDestination).GetProperties())
        {
            if (!destProp.CanWrite) continue;

            var sourceProp = sourceProps.FirstOrDefault(p =>
                p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);

            if (sourceProp is not null)
                destProp.SetValue(destination, sourceProp.GetValue(source));
        }

        return destination;
    }
}