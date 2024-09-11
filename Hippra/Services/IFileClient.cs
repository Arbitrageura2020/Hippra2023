using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hippra.Services
{
    public interface IFileClient
    {
        Task<bool> DeleteFile(string storeName, string filePath);
        Task<bool> FileExists(string storeName, string filePath);
        Task<Stream> GetFile(string storeName, string filePath);
        Task<string> GetFileUrl(string storeName, string filePath);
        Task<FileStream> GetFileAsFilStream(string storeName, string filePath);
        Task SaveFile(string storeName, string filePath, Stream fileStream);
        Task SaveFile(string storeName, string filePath, byte[] byteArray);
    }
}
