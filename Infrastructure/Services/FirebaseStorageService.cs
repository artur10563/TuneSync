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

		//TODO: save to database , test if metadata can pass info
		/// <summary>
		/// Uploads file and return its downloading path
		/// </summary>
		/// <param name="fileStream">File as fileStream</param>
		/// <returns>Path from which the file can be downloaded</returns>
		public async Task<string> UploadFile(Stream fileStream)
		{
			//Guid is used for name as files will get replaced if name is the same
			var fileName = Guid.NewGuid().ToString() + ".mp3";
			var uploadTask = _fileStorage.Child(fileName).PutAsync(fileStream: fileStream);

			return await uploadTask;
		}

		//TODO: Test
		public async Task<string> Get(string filePath)
		{

			var fileUrl = await _fileStorage.Child(filePath).GetDownloadUrlAsync();
			return fileUrl;
		}
	}
}
