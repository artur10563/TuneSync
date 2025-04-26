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
        public async Task<string> UploadFileAsync(Stream fileStream, StorageFolder folder)
        {
            var guid = NewGuid();
            var folderName = folder.GetPath();
            switch (folder)
            {
                case StorageFolder.Audio:
                    await _fileStorage.Child(folderName).Child(guid + ".mp3").PutAsync(fileStream: fileStream);
                    return guid.ToString();
                case StorageFolder.Images:
                    await _fileStorage.Child(folderName).Child(guid + ".jpg").PutAsync(fileStream);
                    break;
                case StorageFolder.None:
                    await _fileStorage.Child(folderName).Child(guid.ToString()).PutAsync(fileStream); //TODO: add extension
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(folder), folder, null);
            }
            
             return folderName + "/" + guid;
        }

        public async IAsyncEnumerable<string> GetFileNames(StorageFolder? folder = null)
        {
            var objects = _storageClient.ListObjectsAsync(_bucketName,folder?.GetPath());
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