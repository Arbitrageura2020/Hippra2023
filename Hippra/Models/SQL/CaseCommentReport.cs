using System;
namespace Hippra.Models.SQL
{
    public class CaseCommentReport
    {
        public int ID { get; set; }
        public long CommentId { get; set; }
        public string UserId { get; set; }
        public long CaseId { get; set; }
        public string ReportText { get; set; }
        public DateTime DateAdded { get; set; }

    }
}
