using Application.Services;
using Domain.Enums;
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
        /// <param name="folder">File folder</param>
        /// <returns>Path from which the file can be downloaded</returns>
        public async Task<string> UploadFileAsync(Stream fileStream, StorageFolder folder = StorageFolder.None)
        {
            var guid = NewGuid();
            switch (folder)
            {
                case StorageFolder.None:
                    await _fileStorage.Child(guid+ ".mp3").PutAsync(fileStream: fileStream);
                    break;
                case StorageFolder.Images:
                    await _fileStorage.Child("images").Child(guid+ ".jpg").PutAsync(fileStream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(folder), folder, null);
            }
            return folder.GetPath().Replace("/", "%2F") + guid;
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