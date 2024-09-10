﻿using Hippra.Models.Enums;
using System;
namespace Hippra.Models.SQL
{
    public class CaseFiles
    {
        public long ID { get; set; }
		public string UploadedByUserId { get; set; }
		public long CaseId { get; set; }
        public string Container { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        
    }
}
