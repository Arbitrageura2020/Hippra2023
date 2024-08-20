using Hippra.Data;
using Hippra.Models.DTO;
using Hippra.Models.POCO;
using Hippra.Models.SQL;
using Hippra.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Services
{
    public class HistoryLogService: IHistoryLogService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        public HttpContext WebContext => _httpContextAccessor.HttpContext;
        public HistoryLogService(IDbContextFactory<ApplicationDbContext> dbFactory, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            DbFactory = dbFactory;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // history type
        public async Task<long> AddHistory(AddHistoryLogDto newHistory)
        {
            using var _context = DbFactory.CreateDbContext();
            var historyLogItem = new HistoryLog();
            historyLogItem.PostID = newHistory.PostID;
            historyLogItem.CommentId = newHistory.CommentId;
            historyLogItem.UserId=newHistory.UserId;
            historyLogItem.Detail = newHistory.Detail;
            historyLogItem.Tag = newHistory.Tag;    
            historyLogItem.Type = newHistory.Type;
            historyLogItem.AddedOn = DateTime.Now;
            _context.HistoryLogs.Add(historyLogItem);
            await _context.SaveChangesAsync();
            return historyLogItem.ID;
        }
        public async Task<HistoryResultModel> GetPostHistories(string posterID, int targetPage, int PageSize)
        {
            using var _context = DbFactory.CreateDbContext();

            List<HistoryLog> histories = await _context.HistoryLogs.Where(c => c.UserId == posterID).OrderByDescending(s => s.AddedOn).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            HistoryResultModel result = new HistoryResultModel();
            result.Histories = histories;
            result.TotalCount = await _context.HistoryLogs.AsNoTracking().CountAsync(s => s.UserId == posterID);
            return result;
        }

        public async Task<HistoryResultModel> GetPostHistories(string posterID)
        {
            using var _context = DbFactory.CreateDbContext();

            List<HistoryLog> histories = await _context.HistoryLogs.Where(c => c.UserId == posterID).OrderByDescending(s => s.AddedOn).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            HistoryResultModel result = new HistoryResultModel();
            result.Histories = histories;
            result.TotalCount = await _context.HistoryLogs.AsNoTracking().CountAsync(s => s.UserId == posterID);
            return result;
        }

        public async Task<HistoryLog> GetHistoryByIDs(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            HistoryLog h = await _context.HistoryLogs.FirstOrDefaultAsync(h => h.ID == id);
            return h;
        }


        //public async Task AddHistoryPost(Case pCase)
        //{
        //    var userInfo = await _userManager.GetUserAsync(WebContext.User);
        //    PostHistory newHistory = new PostHistory();
        //    List<Follow> followerList = new List<Follow>();
        //    int lastHistoryId = -1;
        //    //history stuffs
        //    newHistory.ID = 0;
        //    newHistory.PostID = pCase.ID;
        //    newHistory.CreationDate = DateTime.Now;
        //    newHistory.PosterID = userInfo.PublicId;
        //    newHistory.UserDisplayName = userInfo.FirstName;
        //    newHistory.Title = pCase.Topic;
        //    newHistory.Detail = pCase.Description;
        //    newHistory.HistoryTypes = "added a new case";
        //    lastHistoryId = await hService.AddHistory(newHistory);
        //    //follow
        //    followerList = await hService.GetAllFollowers(userInfo.PublicId);
        //    foreach (var follower in followerList)
        //    {
        //        Notification newNotifs = new Notification();
        //        newNotifs.ID = 0;
        //        //newNotifs.SenderID = follower.FollowingUserID;
        //        //newNotifs.ReceiverID = follower.FollowerUserID;
        //        //newNotifs.NotificationID = lastHistoryId;
        //        //newNotifs.IsRead = -1;
        //        newNotifs.IsResponseNeeded = -1;
        //        newNotifs.CreationDate = DateTime.Now;
        //        await hService.AddNotification(newNotifs);
        //    }
        //}


        public async Task AddToHistory(string historyType, AddEditCaseViewModel caseItem)
        {
            //history stuffs
            //PostHistory newHistory = new PostHistory();
            //newHistory.ID = 0;
            //newHistory.PostID = caseItem.ID;
            //newHistory.CreationDate = DateTime.Now;
            //newHistory.PosterID = caseItem.PosterID;
            ////  newHistory.UserDisplayName = fullName;
            //newHistory.Title = caseItem.Topic;
            //newHistory.Detail = caseItem.Description;
            //newHistory.HistoryTypes = historyType;
            //await this.AddHistory(newHistory).ConfigureAwait(false);
        }



        public async Task addToHistoryComment(string historyType, Case c)
        {
            //history stuffs
            AddHistoryLogDto newHistory = new AddHistoryLogDto();
            newHistory.PostID = c.ID;
            newHistory.UserId = c.UserId;
            //newHistory.UserDisplayName = userInfo.FullName;
            //newHistory.UserDisplayName2 = c.PosterName;
            newHistory.Type = Models.Enums.HistoryLogType.NewCommentAdded;
            newHistory.Detail = c.Description;
            await this.AddHistory(newHistory);
        }
    }
}
