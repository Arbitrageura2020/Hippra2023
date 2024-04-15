using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
	public class CaseComment
	{
		public int ID { get; set; }

		public int PosterId { get; set; }
		public string PosterName { get; set; }
		public string posterSpeciality { get; set; }
		public string Comment { get; set; }
		public DateTime PostedDate { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public string imgUrl { get; set; }
		public int CaseID { get; set; }
		public int VoteUp { get; set; }
		public int VoteDown { get; set; }
		public string ProfileUrl = "";
		public Case Case { get; set; }
		public string UserId { get; set; }
		public AppUser User { get; set; }
	}
}
