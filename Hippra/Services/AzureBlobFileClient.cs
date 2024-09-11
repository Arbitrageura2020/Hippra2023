
using Azure.Storage.Blobs;
using Hippra.Models.ViewModel;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Hippra.Services
{
    public class AzureBlobFileClient : IFileClient
    {
        private BlobServiceClient _blobClient;

        public AzureBlobFileClient(string connectionString)
        {
            _blobClient = new BlobServiceClient(connectionString);
        }

        public async Task<bool> DeleteFile(string storeName, string filePath)
        {
            try
            {
                var container = _blobClient.GetBlobContainerClient(storeName);
                var blob = container.GetBlobClient(filePath.ToLower());
                return await blob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                var t = 1;
                return false;
            }
        }

        public async Task<bool> FileExists(string storeName, string filePath)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);
            var blob = container.GetBlobClient(filePath.ToLower());

            return await blob.ExistsAsync();
        }

        public async Task<Stream> GetFile(string storeName, string filePath)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);

            var blob = container.GetBlobClient(filePath.ToLower());

            var mem = new MemoryStream();
            // Check if the file exists in the container
            if (await blob.ExistsAsync())
            {
                await blob.DownloadToAsync(mem);
                mem.Seek(0, SeekOrigin.Begin);
            }

            return mem;
        }

        public async Task<FileStream> GetFileAsFilStream(string storeName, string filePath)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);
            var blob = container.GetBlobClient(filePath.ToLower());

            var fileStream = new FileStream(Path.GetFileName(filePath), FileMode.OpenOrCreate);
            await blob.DownloadToAsync(fileStream);
            fileStream.Close();

            return fileStream;
        }

        public async Task<string> GetFileUrl(string storeName, string filePath)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);
            var blob = container.GetBlobClient(filePath.ToLower());
            string url = null;

            if (await blob.ExistsAsync())
            {
                url = blob.Uri.AbsoluteUri;
            }

            return url;
        }

        public async Task SaveFile(string storeName, string filePath, Stream fileStream)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);
            var blob = container.GetBlobClient(filePath.ToLower());
            try
            {
                var result=await blob.UploadAsync(fileStream, true);
                // Open a stream for the file we want to upload
            }
            catch (Exception e)
            {
            }
           
        }

        public async Task SaveFile(string storeName, string filePath, byte[] byteArray)
        {
            var container = _blobClient.GetBlobContainerClient(storeName);
            var blob = container.GetBlobClient(filePath.ToLower());
            try
            {
                BinaryData binaryData = new BinaryData(byteArray);

                await blob.UploadAsync(binaryData, true);
            }
            catch (Exception e)
            {
            }
        }
    }
}
