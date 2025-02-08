using Domain.Primitives;

namespace Application.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        
        return query.Skip((page - 1) * GlobalVariables.PaginationConstants.PageSize).Take(GlobalVariables.PaginationConstants.PageSize);
    }
}