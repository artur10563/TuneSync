using System.Linq.Expressions;
using Domain.Entities;
using Domain.Primitives;

namespace Application.Extensions;


public sealed record SongQueryInfo(int TotalCount, TimeSpan TotalLength);

public static class MetadataExtension
{
    public static SongQueryInfo GetSongsInfo(this IQueryable<Song> songQuery)
    {
        var info = songQuery
            .GroupBy(_ => 1)
            .Select(g => new SongQueryInfo(
                g.Count(),
                TimeSpan.FromSeconds(g.Sum(song => song.AudioLength.TotalSeconds))
            )).First();

        return info;
    }
    
    public static Dictionary<string, object> ToMetadataDictionary(this SongQueryInfo info)
    {
        var metadata = new Dictionary<string, object>()
        {
            { GlobalVariables.MetadataKeys.TotalLength, info.TotalLength },
        };

        return metadata;
    }

    #region Object to dictionary
    //Not being used but might be useful if we move to Attribute driven logic 
    
    public static Dictionary<string, object> ToMetadataDictionary<T>(this T sourceObject, params Expression<Func<T, object>>[] selectors) where T : class
    {
        var props = sourceObject.GetType().GetProperties()
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType));

        if (selectors.Length != 0)
        {
            var propNames = selectors
                .Select(expr =>
                {
                    if (expr.Body is MemberExpression m)
                        return m.Member.Name;
                    if (expr.Body is UnaryExpression u && u.Operand is MemberExpression um)
                        return um.Member.Name;
                    throw new ArgumentException("Invalid expression");
                })
                .ToArray();
            
            props = props.Where(prop => propNames.Contains(prop.Name));
            
        }

        
        return props
            .ToDictionary(
                prop => prop.Name,
                prop => prop.GetValue(sourceObject));
    }
    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(TimeSpan) ||
            type == typeof(Guid);
    }

    #endregion
}