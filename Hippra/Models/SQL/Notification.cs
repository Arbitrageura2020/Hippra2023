using Hippra.Models.Enums;
using System;
namespace Hippra.Models.SQL
{
    public class Notification
    {
        public long ID { get; set; }
        public string SenderUserId { get; set; }
        public AppUser SenderUser{get;set; }
        public string ReceiverUserID { get; set; }
        public int IsResponseNeeded { get; set; }
        public NotificationType Type { get; set; } = NotificationType.AddedComment;
        public bool IsNotificationRead { get; set; }
        public DateTime CreationDate { get; set; }
        public long CaseId { get; set; }
        public Case Case { get; set; }
        public long CommentId { get; set; }

    }
}
