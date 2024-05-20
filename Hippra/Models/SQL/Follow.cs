using System;
namespace Hippra.Models.SQL
{
    public class Follow
    {
        public long ID { get; set; }
        public string FollowerUserID { get; set; }
        public string FollowingUserID { get; set; }
    }
}
