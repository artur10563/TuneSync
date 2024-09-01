namespace Application.Services
{
	public interface IFirebaseStorageService
	{
		Task<string> Get(string fileName);
		Task<string> UploadFile(Stream fileStream);
	}
}
