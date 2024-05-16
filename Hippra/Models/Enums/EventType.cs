using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum EventType
    {
		[Display(Name = "Ask for help")]
		CommentAdded =0,
		[Display(Name = "Clnical Knowledge")]
		Followed=1,
        [Display(Name = "Clnical Knowledge")]
        NewPostAdded = 2,
    }
}
