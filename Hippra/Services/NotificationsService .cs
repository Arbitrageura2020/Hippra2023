using Hippra.Data;
using Hippra.Extensions;
using Hippra.Models.POCO;
using Hippra.Models.SQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hippra.Models.FTDesign;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;
using Newtonsoft.Json;
using FTEmailService;
using Microsoft.EntityFrameworkCore;
using Hippra.Models.Enums;
using Hippra.Code;
using Hippra.API;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Hippra.Models.ViewModel;
using Hippra.Components;
using Hippra.Models.DTO;

namespace Hippra.Services
{
    public class NotificationsService : INotificationsService
    {
        private IDbContextFactory<ApplicationDbContext> DbFactory;

        public NotificationsService(
            IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            DbFactory = dbFactory;
        }

        //Notification
        public async Task<bool> AddNotification(AddNotificationDto request)
        {
            using var _context = DbFactory.CreateDbContext();
            if (request != null)
            {
                if (request.Type == NotificationType.AddedComment||request.Type==NotificationType.AddedUpVote)
                {
                    var receiverId = _context.Cases.Where(x => x.ID == request.PostID).Select(x => x.UserId).AsNoTracking().FirstOrDefault();
                    if (receiverId != null)
                    {
                        var notification = new Notification()
                        {
                            SenderUserId = request.SenderUserID,
                            CreationDate = DateTime.UtcNow,
                            CaseId = request.PostID,
                            Type = request.Type,
                            ReceiverUserID = receiverId,
                            CommentId = request.CommentId,
                        };
                        _context.Notifications.Add(notification);
                    }
                }

                if (request.Type == NotificationType.Followed)
                {

                    var notification = new Notification()
                    {
                        SenderUserId = request.SenderUserID,
                        CreationDate = DateTime.UtcNow,
                        CaseId = request.PostID,
                        Type = request.Type,
                        ReceiverUserID = request.ReceiverUserID
                    };
                    _context.Notifications.Add(notification);

                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var t = ex;
                    return false;
                }
                return true;
            }
            return false;
        }

        //public async Task<NotificationResultModel> GetAllNotifications(string userID, int targetPage, int PageSize)
        //{
        //    using var _context = DbFactory.CreateDbContext();

        //    List<Notification> ListNotifs = await _context.Notifications.Where(n => n.ReceiverUserID == userID).OrderByDescending(n => n.CreationDate).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
        //    //var h = histories.OrderByDescending(h => h.CreationDate);
        //    NotificationResultModel result = new NotificationResultModel();
        //    result.Notifications = ListNotifs;
        //    result.TotalCount = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverUserID == userID);
        //    return result;
        //}

        public async Task<IList<NotificationDto>> GetAllNotificationsForUser(string userID)
        {
            using var _context = DbFactory.CreateDbContext();

            List<NotificationDto> result = await _context.Notifications.Where(n => n.ReceiverUserID == userID)
                .Select(x => new NotificationDto()
                {
                    CommentId = x.CommentId,
                    CreationDate = x.CreationDate,
                    ID = x.ID,
                    IsNotificationRead = x.IsNotificationRead,
                    IsResponseNeeded = x.IsResponseNeeded,
                    PostID = x.CaseId,
                    Type = x.Type,
                    ReceiverUserID = x.ReceiverUserID,
                    SenderUserID = x.SenderUserId,
                    PostTitle = x.Case.Topic,
                    SenderImage = x.SenderUser.ProfileUrl
                }).OrderByDescending(n => n.CreationDate).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<int> CountMyNotification(string userID)
        {
            using var _context = DbFactory.CreateDbContext();

            int count = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverUserID == userID && !s.IsNotificationRead);
            return count;
        }

        public async Task<bool> DeleteNotification(long id)
        {
            using var _context = DbFactory.CreateDbContext();

            Notification notif = await _context.Notifications.FirstOrDefaultAsync(n => n.ID == id);
            if (notif != null)
            {
                _context.Notifications.Remove(notif);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> NotificationRead(long id)
        {
            using var _context = DbFactory.CreateDbContext();

            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.ID == id);

            if (notif == null)
            {
                return false;
            }

            notif.IsNotificationRead = true;
            try
            {
                _context.Update(notif);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(notif.ID))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
        private bool NotificationExists(long id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Notifications.AsNoTracking().Any(n => n.ID == id);
        }

    }
}
