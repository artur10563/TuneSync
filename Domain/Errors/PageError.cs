using Domain.Primitives;

namespace Domain.Errors;

public static class PageError
{
    public static Error InvalidPage => new Error($"Minimum page is {GlobalVariables.PaginationConstants.PageMin}");
}