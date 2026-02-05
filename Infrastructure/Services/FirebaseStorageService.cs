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
        public async Task<FileUploadResult> UploadFileAsync(Stream fileStream, StorageFolder folder)
        {
            //returned Guid for audio, folderName/Guid for other jpg and other files?
            var guid = NewGuid();
            var folderName = folder.GetPath();
            
            var objectName = folder switch 
            {
                StorageFolder.Audio => $"{folderName}/{guid}.mp3",
                StorageFolder.Images => $"{folderName}/{guid}.jpg",
                _ => $"{StorageFolder.Dump.GetPath()}/{guid}"
            };
            
            var newObject = new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName
            };

            await _storageClient.UploadObjectAsync(
                newObject,
                fileStream
            );
            
            _logger.Log($"File uploaded to Firebase", LogLevel.Information, new { guid, objectName });

            return new FileUploadResult(guid, objectName);
        }

        public async IAsyncEnumerable<string> GetFileNames(StorageFolder? folder = null)
        {
            var objects = _storageClient.ListObjectsAsync(_bucketName, folder?.GetPath());
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