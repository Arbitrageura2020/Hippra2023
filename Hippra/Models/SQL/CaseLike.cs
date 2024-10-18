using System;
namespace Hippra.Models.SQL
{
    public class CaseLike
    {
        public long ID { get; set; }
        public long CaseId { get; set; }
        public string LikedByUserId { get; set; }
        public DateTime LikeDate { get; set; }
        
    }
}
