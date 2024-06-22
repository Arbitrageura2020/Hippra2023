using System;
namespace Hippra.Models.SQL
{
    public class CaseCommentVote
    {
        public int ID { get; set; }
        public long CommentId { get; set; }
        public int PosterID { get; set; }
		public string UserId { get; set; }
		public int VoterID { get; set; }
        public DateTime VoteDate { get; set; }
        
    }
}
