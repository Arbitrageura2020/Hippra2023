using System;
namespace Hippra.Models.SQL
{
    public class CaseCommentVote
    {
        public long ID { get; set; }
        public long CaseCommentId { get; set; }
        public CaseComment CaseComment { get; set; }
        public string UserId { get; set; }
        public DateTime VoteDate { get; set; }
        
    }
}
