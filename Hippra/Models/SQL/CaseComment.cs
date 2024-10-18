using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class CaseComment
    {
        public long ID { get; set; }
        public string Comment { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public long CaseID { get; set; }
        public int VoteUp { get; set; }
        public Case Case { get; set; }
        public bool IsAnonymus { get; set; } = false;

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public IList<CaseCommentVote> CaseCommentVotes { get; set; }
        public IList<CaseCommentFile> Files { get; set; }
    }
}
