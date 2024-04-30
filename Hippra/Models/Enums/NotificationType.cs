using Hippra.Models.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Hippra.Models.Enums
{
    public enum NotificationType
    {
		[Display(Name = "added a comment on")]
		AddedComment =0,
		[Display(Name = "updated a comment on")]
		EditedComment=1,

//added a comment on
//added a new case
//added an up vote on
//connected with
//deleted a comment on
//edited a case
//edited profile
//followed
//invited
//joined Hippra
//searched
//updated a comment on
//updated background picture
//updated biography
//updated profile pricture
}
}
