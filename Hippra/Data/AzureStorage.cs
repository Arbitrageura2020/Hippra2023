using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Diagnostics;
using System.IO;

namespace Hippra.Data
{
    public class AzureStorage
    {
        private AppSettings AppSettings { get; set; }

        CloudStorageAccount storageAccount = null;
        CloudBlobContainer chatRootContainer = null;


        // error code
        private string errCodeRootContainerNotFound = "Root Container Not Found!";


        // alpha code
        //CloudBlobContainer privateCloudBlobContainer = null;
        //CloudBlobContainer publicCloudBlobContainer = null;
        //string sourceFile = null;
        //string destinationFile = null;

        // Retrieve the connection string for use with the application. The storage connection string is stored
        // in an environment variable on the machine running the application called storageconnectionstring.
        // If the environment variable is created after the application is launched in a console or with Visual
        // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
        string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring"); //UseDevelopmentStorage=true

        public AzureStorage(IOptions<AppSettings> settings)
        {
            AppSettings = settings.Value;
        }

        public bool CheckAzureStorage()
        {
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(AppSettings.StorageConnectionString, out storageAccount))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> InitChatRootContainer()
        {
            bool success = false;
            if (CloudStorageAccount.TryParse(AppSettings.StorageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                    chatRootContainer = cloudBlobClient.GetContainerReference(AppSettings.StorageRootContainer);// + Guid.NewGuid().ToString());                    
                    await chatRootContainer.CreateIfNotExistsAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await chatRootContainer.SetPermissionsAsync(permissions);

                    success = true;
                }
                catch (StorageException ex)
                {
                    Debug.WriteLine("Error returned from the service: {0}", ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
            return success;
        }

        public async Task SetBlob(string blobName, string data)
        {
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return;
                }
            }
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(blobName);
            await blockBlob.UploadTextAsync(data);
        }
        public async Task<string> GetBlob(string blobName)
        {
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return "";
                }
            }
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(blobName);

            var exists = await blockBlob.ExistsAsync();
            if (exists)
                return await blockBlob.DownloadTextAsync();

            return string.Empty;
        }
        public async Task SetBlobFile(string blobName, Stream fileStream)
        {
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return;
                }
            }
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(blobName);
            //await blockBlob.UploadTextAsync(data);
            // Upload the file
            await blockBlob.UploadFromStreamAsync(fileStream);
        }


        public async Task<Stream> GetBlobFile(string blobName)
        {
            MemoryStream memstream = new MemoryStream();
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return null;
                }
            }
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(blobName);

            var exists = await blockBlob.ExistsAsync();
            if (exists)
            {
                //https://stackoverflow.com/questions/28526249/azure-downloadtostreamasync-method-hangs
                await blockBlob.DownloadToStreamAsync(memstream).ConfigureAwait(false);
                return memstream;
            }


            return null;
        }
        public async Task<string> DeleteBlob(string fileName)
        {
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return "";
                }
            }
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(fileName);
            var exists = await blockBlob.ExistsAsync();
            if (exists)
            {
                await blockBlob.DeleteAsync();
            }

            return "";
        }
        public async Task<string> GetBlobReference()
        {
            if (!CheckChatRootContainer())
            {
                await InitChatRootContainer();
                if (!CheckChatRootContainer())
                {
                    Debug.WriteLine(errCodeRootContainerNotFound);
                    return "";
                }
            }

            return chatRootContainer.Uri.ToString();

        }

        // private 
        private bool CheckChatRootContainer()
        {
            if (chatRootContainer == null)
            {
                return false;
            }
            return true;
        }

        public async Task SaveFileToBlob(string container,string blobName, Stream fileStream)
        {
           
            CloudBlockBlob blockBlob = chatRootContainer.GetBlockBlobReference(blobName);
            //await blockBlob.UploadTextAsync(data);
            // Upload the file
            await blockBlob.UploadFromStreamAsync(fileStream);
        }
    }
}
