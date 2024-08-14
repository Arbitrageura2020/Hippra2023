using Hippra.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.ViewModel
{
    public class ReportCommentViewModel
    {
        public long CommentId { get; set; }

        public string UserId { get; set; }
        public string ReportText { get; set; }
      
    }
}
