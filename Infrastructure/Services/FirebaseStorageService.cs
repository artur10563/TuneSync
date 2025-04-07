using Application.Services;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using Google.Cloud.Storage.V1;
using static System.Guid;

namespace Infrastructure.Services
{
    public class FirebaseStorageService : IStorageService
    {
        private readonly FirebaseStorage _fileStorage;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILoggerService _logger;

        public FirebaseStorageService(IConfiguration configuration, ILoggerService logger, StorageClient storageClient)
        {
            _logger = logger;
            _storageClient = storageClient;
            _bucketName = configuration["FirestoreStorage:DefaultBucket"];
            _fileStorage = new FirebaseStorage(_bucketName);
        }

        /// <summary>
        /// Uploads file and return its Guid
        /// </summary>
        /// <param name="fileStream">File as fileStream</param>
        /// <returns>Path from which the file can be downloaded</returns>
        public async Task<Guid> UploadFileAsync(Stream fileStream)
        {
            var guid = NewGuid();
            var fileName = guid.ToString() + ".mp3";
            await _fileStorage.Child(fileName).PutAsync(fileStream: fileStream);

            return guid;
        }

        public async IAsyncEnumerable<string> GetFileNames()
        {
            var objects = _storageClient.ListObjectsAsync(_bucketName);
            await foreach (var item in objects)
            {
                yield return item.Name;
            }
        }

        public async Task<bool> TryDeleteFileAsync(string fileName)
        {
            try
            {
                await _fileStorage.Child(fileName).DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.Log("Failed file deletion", LogLevel.Warning, fileName, e);
                return false;
            }
        }
    }
}