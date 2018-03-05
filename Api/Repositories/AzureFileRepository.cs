using Api.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace Api.Repositories
{
    public class AzureFileRepository : IFileRepository
    {
        private readonly IOptions<FileSettings> _fileSettings;

        public AzureFileRepository(IOptions<FileSettings> fileSettings)
        {
            _fileSettings = fileSettings;
        }

        public byte[] Get(string fileName)
        {
            StorageCredentials storageCredentials = new StorageCredentials("myAccountName", "myAccountKey");
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = cloudBlobClient.GetContainerReference("mycontainer");
            return null;
        }

        public bool Add(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            // Create directory if it doesn't already exist
            FileInfo fileInfo = new FileInfo(_fileSettings.Value.Path);
            fileInfo.Directory.Create();

            // Use Combine to add the file name to the path
            string filePath = Path.Combine(_fileSettings.Value.Path, file.FileName);

            // Write the file to the file path.
            // This will overwrite the file if it already exists.
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return true;
        }

        public bool Delete(string fileName)
        {
            string filePath = Path.Combine(_fileSettings.Value.Path, fileName);

            File.Delete(filePath);

            return true;
        }
    }
}
