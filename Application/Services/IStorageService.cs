using Domain.Enums;

namespace Application.Services
{
	public interface IStorageService
	{
		Task<string> UploadFileAsync(Stream fileStream, StorageFolder folder = StorageFolder.None);
		Task<bool> TryDeleteFileAsync(string fileName);
		IAsyncEnumerable<string> GetFileNames();
	}
}
