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
        public long CaseID { get; set; }
        public int Votes { get; set; }
        public bool IsAnonymos { get; set; }
        public bool IsMyOwnComment { get; set; }
        public bool VotedByCurrentUser { get; set; }
        public string TimeDifferenceText
        {
            get
            {
                string result = string.Empty;
                var timeSpan = DateTime.UtcNow.Subtract(this.PostedDate);

                if (timeSpan <= TimeSpan.FromSeconds(60))
                {
                    result = string.Format("{0} seconds ago", Math.Abs(timeSpan.Seconds));
                }
                else if (timeSpan <= TimeSpan.FromMinutes(60))
                {
                    result = timeSpan.Minutes > 1 ?
                        String.Format("about {0} minutes ago", Math.Abs(timeSpan.Minutes)) :
                        "about a minute ago";
                }
                else if (timeSpan <= TimeSpan.FromHours(24))
                {
                    result = timeSpan.Hours > 1 ?
                        String.Format("about {0} hours ago", Math.Abs(timeSpan.Hours)) :
                        "about an hour ago";
                }
                else if (timeSpan <= TimeSpan.FromDays(30))
                {
                    result = timeSpan.Days > 1 ?
                        String.Format("about {0} days ago", Math.Abs(timeSpan.Days)) :
                        "yesterday";
                }
                else if (timeSpan <= TimeSpan.FromDays(365))
                {
                    result = timeSpan.Days > 30 ?
                        String.Format("about {0} months ago", Math.Abs(timeSpan.Days / 30)) :
                        "about a month ago";
                }
                else
                {
                    result = timeSpan.Days > 365 ?
                        String.Format("about {0} years ago", Math.Abs(timeSpan.Days / 365)) :
                        "about a year ago";
                }

                return result;
            }
        }
        public AppUser User { get; set; }
        public IList<CaseCommentFile> Files { get; set; }
    }
}
