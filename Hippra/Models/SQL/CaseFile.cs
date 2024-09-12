using Hippra.Models.Enums;
using System;
namespace Hippra.Models.SQL
{
    public class CaseFile
    {
        public long ID { get; set; }
		public string UploadedByUserId { get; set; }
		public int CaseID { get; set; }
        public Case Case { get; set; }
        public string Container { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        
    }
}
