namespace Domain.Errors
{
	public static class SongError
	{
		public static Error InvalidLength => new Error("Max length is {0}");
		public static Error InvalidSize => new Error("Max file size is {0}");
	}
}
