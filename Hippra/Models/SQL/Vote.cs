using System;
namespace Hippra.Models.SQL
{
    public class Vote
    {
        public int ID { get; set; }
        public int CID { get; set; }
        public int VoteType { get; set; }
        public int PosterID { get; set; }
		public string UserId { get; set; }
		public int VoterID { get; set; }
        public DateTime VoteDate { get; set; }
        
    }
}
