using System.IO;

namespace Hippra.Models.DTO
{
    public class FileDownloadResult
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Stream FileContent { get; set; }
    }
}
