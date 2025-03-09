using Domain.Primitives;

namespace Application.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize = GlobalVariables.PaginationConstants.PageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}