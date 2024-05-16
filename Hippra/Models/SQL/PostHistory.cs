using System;
using System.Collections.Generic;
namespace Hippra.Models.SQL
{
    public class PostHistory
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int PosterID { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserID { get; set; }
        public string UserDisplayName { get; set; }
        public string UserDisplayName2 { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string HistoryTypes { get; set; }
        public string IsRead = ""; //for notification
        public long NotificationID { get; set; }

    }
}
