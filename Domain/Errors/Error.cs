using Domain.Extensions;

namespace Domain.Errors
{
    public sealed record Error(string Description)
    {
        public static readonly Error None = new(string.Empty);
        public static readonly Error AccessDenied = new($"Access denied");
        public static Error NotFound(string entity) => new($"{entity.Capitalize()} Not Found");
        public static Error Exists(string entity) => new($"{entity.Capitalize()} Already Exists");
        public static Error Required(string entity) => new($"{entity.Capitalize()} is Required");
        public static readonly  Error SomethingWrong = new("Something went wrong..."); // When we don't know what caused the error
    }
}
