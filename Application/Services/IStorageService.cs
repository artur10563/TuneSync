namespace Application.Services
{
	public interface IStorageService
	{
		Task<Guid> UploadFileAsync(Stream fileStream);
	}
}
