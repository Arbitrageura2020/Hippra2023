using System;
namespace Hippra.Models.SQL
{
    public class CaseLike
    {
        public int ID { get; set; }
        public int CaseId { get; set; }
        public string LikedByUserId { get; set; }
        public DateTime LikeDate { get; set; }
        
    }
}
