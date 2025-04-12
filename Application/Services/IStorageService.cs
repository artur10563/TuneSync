using Domain.Enums;

namespace Application.Services
{
	public interface IStorageService
	{
		Task<string> UploadFileAsync(Stream fileStream, StorageFolder folder);
		Task<bool> TryDeleteFileAsync(string fileName);
		IAsyncEnumerable<string> GetFileNames(StorageFolder? folder = null);
	}
}
