namespace VIAPadelClub.Core.Tools.ObjectMapper;

public interface IMapping
{
}
public interface IMapping<TSource, TDestination> : IMapping
{
    TDestination Convert(TSource source);
}