using Hippra.Data;
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
        public async Task<int> AddHistory(PostHistory newHistory)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.PostHistories.Add(newHistory);
            await _context.SaveChangesAsync();
            return newHistory.ID;
        }
        public async Task<HistoryResultModel> GetPostHistories(int posterID, int targetPage, int PageSize)
        {
            using var _context = DbFactory.CreateDbContext();

            List<PostHistory> histories = await _context.PostHistories.Where(c => c.PosterID == posterID).OrderByDescending(s => s.CreationDate).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            HistoryResultModel result = new HistoryResultModel();
            result.Histories = histories;
            result.TotalCount = await _context.PostHistories.AsNoTracking().CountAsync(s => s.PosterID == posterID);
            return result;
        }

        public async Task<HistoryResultModel> GetPostHistories(int posterID)
        {
            using var _context = DbFactory.CreateDbContext();

            List<PostHistory> histories = await _context.PostHistories.Where(c => c.PosterID == posterID).OrderByDescending(s => s.CreationDate).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            HistoryResultModel result = new HistoryResultModel();
            result.Histories = histories;
            result.TotalCount = await _context.PostHistories.AsNoTracking().CountAsync(s => s.PosterID == posterID);
            return result;
        }

        public async Task<PostHistory> GetHistoryByIDs(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            PostHistory h = await _context.PostHistories.FirstOrDefaultAsync(h => h.ID == id);
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
            PostHistory newHistory = new PostHistory();
            newHistory.ID = 0;
            newHistory.PostID = caseItem.ID;
            newHistory.CreationDate = DateTime.Now;
            newHistory.PosterID = caseItem.PosterID;
            //  newHistory.UserDisplayName = fullName;
            newHistory.Title = caseItem.Topic;
            newHistory.Detail = caseItem.Description;
            newHistory.HistoryTypes = historyType;
            await this.AddHistory(newHistory).ConfigureAwait(false);
        }

        public async Task<int> addToHistory(string historyType, AppUser user)
        {
            //history stuffs
            PostHistory newHistory = new PostHistory();
            newHistory.ID = 0;
            newHistory.PostID = user.PublicId;
            newHistory.CreationDate = DateTime.Now;
            newHistory.PosterID = user.PublicId;
            newHistory.UserID = user.PublicId;
            newHistory.UserDisplayName = user.FirstName;
            newHistory.UserDisplayName2 = user.FirstName + " " + user.LastName;
            if (historyType == "Invited")
            {
                newHistory.Title = user.FirstName + " " + user.LastName + "to connect.";
            }
            else
            {
                newHistory.Title = "on " + user.FirstName + " " + user.LastName + "'s Hippra page";
            }

            newHistory.Detail = "";
            newHistory.HistoryTypes = historyType;
            return await this.AddHistory(newHistory);
        }

        public async Task addToHistoryComment(string historyType, Case c)
        {
            var userInfo = await _userManager.GetUserAsync(WebContext.User);
            //history stuffs
            PostHistory newHistory = new PostHistory();
            newHistory.ID = 0;
            newHistory.PostID = c.ID;
            newHistory.UserID = userInfo.PublicId;
            newHistory.CreationDate = DateTime.Now;
            newHistory.PosterID = userInfo.PublicId;
            //newHistory.UserDisplayName = userInfo.FullName;
            //newHistory.UserDisplayName2 = c.PosterName;
            newHistory.Title = c.Topic;
            newHistory.Detail = c.Description;
            newHistory.HistoryTypes = historyType;
            await this.AddHistory(newHistory);
        }
    }
}
