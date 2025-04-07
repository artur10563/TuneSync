namespace Application.Services
{
	public interface IStorageService
	{
		Task<Guid> UploadFileAsync(Stream fileStream);
		Task<bool> TryDeleteFileAsync(string fileName);
		IAsyncEnumerable<string> GetFileNames();
	}
}
