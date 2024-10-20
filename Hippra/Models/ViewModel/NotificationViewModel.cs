﻿using Hippra.Models.DTO;
using Hippra.Models.Enums;
using Hippra.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hippra.Models.ViewModel
{
    public class NotificationViewModel
    {
        public long ID { get; set; }
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }
        public int IsResponseNeeded { get; set; }
        public NotificationType Type { get; set; } = NotificationType.AddedComment;
        public bool IsNotificationRead { get; set; }
        public DateTime CreationDate { get; set; }
        public long PostID { get; set; }
        public long CommentId { get; set; }

        public string PostTitle { get; set; }
        public string SenderImage { get; set; }

        public bool IsGeneralNotification
        {
            get
            {
                var isGeneral = true;
                if (this.Type == NotificationType.AddedComment ||
                    this.Type == NotificationType.AddedUpVote ||
                    this.Type == NotificationType.PostLiked)
                    isGeneral = false;
                return isGeneral;

            }
        }

        public string NavigationLink
        {
            get
            {
                var link = "#";
                if (Type == NotificationType.AddedComment)
                {
                    link = "/viewcase/" + this.PostID+"#comment"+this.CommentId;
                }
                if (Type == NotificationType.AddedUpVote)
                {
                    link = "/viewcase/" + this.PostID + "#comment" + this.CommentId;
                }
                if (Type == NotificationType.Followed)
                {
                    link = "/profile/" + this.SenderUserID;
                }
                if (Type == NotificationType.PostLiked)
                {
                    link = "/viewcase/" + this.PostID;
                }
                return link;
            }
        }

        public static NotificationViewModel FromEntity(NotificationDto entity)
        {

            NotificationViewModel result = new NotificationViewModel
            {
                CommentId = entity.CommentId,
                CreationDate = entity.CreationDate,
                ID = entity.ID,
                IsNotificationRead = entity.IsNotificationRead,
                IsResponseNeeded = entity.IsResponseNeeded,
                PostID = entity.PostID,
                Type = entity.Type,
                ReceiverUserID = entity.ReceiverUserID,
                SenderUserID = entity.SenderUserID,
                PostTitle = entity.PostTitle,
                SenderImage = entity.SenderImage,
            };
            return result;
        }

        public static IList<NotificationViewModel> FromEntityList(ICollection<NotificationDto> items)
        {
            return items.Select(entity => FromEntity(entity)).ToList();
        }
    }
}
