using Domain.Extensions;

namespace Domain.Errors
{
    public sealed record Error(string Description)
    {
        public static readonly Error None = new(string.Empty);
        public static readonly Error AccessDenied = new($"Access denied");
        public static Error NotFound(string entity) => new($"{entity.Capitalize()} Not Found");
    }
}
