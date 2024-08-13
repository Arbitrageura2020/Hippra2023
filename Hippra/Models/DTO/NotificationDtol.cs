using Hippra.Models.Enums;
using System;

namespace Hippra.Models.ViewModel
{
    public class NotificationDto
    {
        public long ID { get; set; }
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }
        public int IsResponseNeeded { get; set; }
        public NotificationType Type { get; set; } = NotificationType.AddedComment;
        public bool IsNotificationRead { get; set; }
        public DateTime CreationDate { get; set; }
        public int PostID { get; set; }
        public long CommentId { get; set; }

        public string PostTitle { get; set; }
        public string SenderImage { get; set; }
    }
}
