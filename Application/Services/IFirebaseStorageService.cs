namespace Application.Services
{
	public interface IFirebaseStorageService
	{
		Task<Guid> UploadFileAsync(Stream fileStream);
	}
}
