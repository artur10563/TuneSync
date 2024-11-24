using Application.Services;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
	public class FirebaseStorageService : IFirebaseStorageService
	{
		private readonly FirebaseStorage _fileStorage;
		public FirebaseStorageService(IConfiguration configuration)
		{
			_fileStorage = new FirebaseStorage(configuration["FirestoreStorage:DefaultBucket"]);
		}

		/// <summary>
		/// Uploads file and return its Guid
		/// </summary>
		/// <param name="fileStream">File as fileStream</param>
		/// <returns>Path from which the file can be downloaded</returns>
		public async Task<Guid> UploadFileAsync(Stream fileStream)
		{
			var guid = Guid.NewGuid();
			var fileName = guid.ToString() + ".mp3";
			await _fileStorage.Child(fileName).PutAsync(fileStream: fileStream);

			return guid;
		}
	}
}
