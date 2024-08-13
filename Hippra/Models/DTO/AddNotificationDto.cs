using Hippra.Models.Enums;
using Hippra.Models.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hippra.Models.DTO
{
    public class AddNotificationDto
    {
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }
        public NotificationType Type { get; set; } = NotificationType.AddedComment;
        public int PostID { get; set; }
        public long CommentId { get; set; }
    }
}
