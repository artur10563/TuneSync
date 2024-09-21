namespace Application.Services
{
	public interface IFirebaseStorageService
	{
		Task<string> GetAsync(string fileName);
		Task<string> UploadFileAsync(Stream fileStream);
	}
}
