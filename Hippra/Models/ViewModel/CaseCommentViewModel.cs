using Hippra.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.ViewModel
{
    public class CaseCommentViewModel
    {
        public long ID { get; set; }

        public string PosterId { get; set; }
        public string PosterName { get; set; }
        public string PosterSpeciality { get; set; }
        public string PosterImage { get; set; } = "/img/hippra/blank-profile.png";
        public string Comment { get; set; }
        public DateTime PostedDate { get; set; }
        public int CaseID { get; set; }
        public int Votes { get; set; }
        public bool IsAnonymos { get; set; }
        public bool IsMyOwnComment { get; set; }
        public string TimeDifferenceText
        {
            get
            {
                var daysDifference = (DateTime.UtcNow - this.PostedDate).Days;
                if (daysDifference < 7)
                    return daysDifference + " Days Ago";
                else
                    return daysDifference/7 + " Weeks Ago";
            }
        }
        public AppUser User { get; set; }
        public IList<CaseCommentFile> Files { get; set; }
    }
}
