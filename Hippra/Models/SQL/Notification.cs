using System;
namespace Hippra.Models.SQL
{
    public class Notification
    {
        public int ID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }
        public int IsRead { get; set; }
        public int NotificationID { get; set; }
        public int IsResponseNeeded { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
