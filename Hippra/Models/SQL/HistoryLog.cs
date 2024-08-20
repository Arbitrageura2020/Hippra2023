using Hippra.Models.Enums;
using System;
using System.Collections.Generic;
namespace Hippra.Models.SQL
{
    public class HistoryLog
    {
        public long ID { get; set; }
        public HistoryLogType Type { get; set; }
        public int PostID { get; set; }
        public long CommentId { get; set; }

        public string UserId { get; set; }

        public string Detail { get; set; }
        public string Tag { get; set; }
        public DateTime AddedOn { get; set; }
        public long NotificationID { get; set; }

    }
}
