using Hippra.Models.Enums;

namespace Hippra.Models.DTO
{
    public class AddHistoryLogDto
    {
        public HistoryLogType Type { get; set; }
        public long PostID { get; set; }
        public long CommentId { get; set; }

        public string UserId { get; set; }

        public string Detail { get; set; }
        public string Tag { get; set; }

        public long NotificationID { get; set; }
    }
}
