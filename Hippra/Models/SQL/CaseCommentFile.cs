using Hippra.Models.Enums;
using System;
namespace Hippra.Models.SQL
{
    public class CaseCommentFile
    {
        public long ID { get; set; }
		public string UploadedByUserId { get; set; }
		public long CaseCommentId { get; set; }
        public CaseComment CaseComment { get; set; }
        public string Container { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        
    }
}
