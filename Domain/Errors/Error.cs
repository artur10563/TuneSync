namespace Domain.Errors
{
	public sealed record Error(string Description)
	{
		public static readonly Error None = new(string.Empty);
		public static readonly Error NotFoundFormat = new($"{0} Not Found");
		public static readonly Error AccessDenied = new($"Access denied");
	}
}
