using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum EventType
    {
		[Display(Name = "Comment Added")]
		CommentAdded =0,
		[Display(Name = "Followed You")]
		Followed=1,
        [Display(Name = "New Post Added")]
        NewPostAdded = 2,
    }
}
