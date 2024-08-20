using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum HistoryLogType
    {
        [Display(Name = "Joined the app")]
        NewUserRegistered = 0,
        [Display(Name = "New post added")]
        NewPostAdded = 1,
        [Display(Name = "Post edited")]
        PostEdited = 2,
        [Display(Name = "New Comment Added")]
        NewCommentAdded = 3,
        [Display(Name = "Edited Comment")]
        EditedComment = 4,
        [Display(Name = "Searched by tag")]
        SearchedByTag = 5,
    }
}
