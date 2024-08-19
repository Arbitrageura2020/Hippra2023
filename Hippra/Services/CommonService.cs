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
using Hippra.Models.ViewModel;
using System.Text.RegularExpressions;
using AutoMapper;
using System.Dynamic;

namespace Hippra.Services
{
    public class CommonService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HippraService hService;
        // private readonly ApplicationDbContext _context;
        private AppSettings AppSettings { get; set; }

        private AzureStorage Storage;
        private ImageHelper ImageHelper;
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        public HttpContext WebContext => _httpContextAccessor.HttpContext;
        public CommonService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            IOptions<AppSettings> settings,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IHttpContextAccessor httpContextAccessor,
            HippraService hService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            AppSettings = settings?.Value;
            // _context = context;
            Storage = new AzureStorage(settings);
            ImageHelper = new ImageHelper(Storage);
            DbFactory = dbFactory;
            _httpContextAccessor = httpContextAccessor;
            this.hService = hService;
        }

        public async Task<List<MedicalSubCategory>> GetAllSubcategories()
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.MedicalSubCategories.AsNoTracking().ToListAsync();

        }

        public async Task<IList<MedicalSubCategory>> GetAllSubcategoriesForCategory(MedicalCategory category)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.MedicalSubCategories.Where(x => x.MedicalCategory == category).AsNoTracking().ToListAsync();

        }

        public async Task<List<Tag>> GetAllTags()
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.Tags.AsNoTracking().ToListAsync();

        }



        //public async Task addNotification(Case caseInfo)
        //{
        //    var userInfo = await _userManager.GetUserAsync(WebContext.User);
        //    bool hasPoster = false;
        //    List<Follow> followerList = new List<Follow>();
        //    followerList = await hService.GetAllFollowers(userInfo.PublicId);
        //    foreach (var follower in followerList)
        //    {
        //        if (follower.FollowerUserID == caseInfo.PosterID || userInfo.PublicId != caseInfo.PosterID)
        //        {
        //            hasPoster = true;
        //        }
        //        Notification newNotifs = new Notification();
        //        newNotifs.ID = 0;
        //       // newNotifs.SenderID = follower.FollowingUserID;
        //       // newNotifs.ReceiverID = follower.FollowerUserID;
        //       //// newNotifs.NotificationID = lastHistoryID;
        //       // newNotifs.IsRead = -1;
        //        newNotifs.IsResponseNeeded = -1;
        //        newNotifs.CreationDate = DateTime.Now;
        //        await hService.AddNotification(newNotifs);
        //    }
        //    if (!hasPoster && userInfo.PublicId != caseInfo.PosterID)
        //    {
        //        Notification newNotifs = new Notification();
        //        newNotifs.ID = 0;
        //      //  newNotifs.SenderID = userInfo.PublicId;
        //      //  newNotifs.ReceiverID = caseInfo.PosterID;
        //      ////  newNotifs.NotificationID = lastHistoryID;
        //      //  newNotifs.IsRead = -1;
        //        newNotifs.IsResponseNeeded = -1;
        //        newNotifs.CreationDate = DateTime.Now;
        //        await hService.AddNotification(newNotifs);
        //    }
        //}

        public bool ContainsOnlyAlphaNumericCharacters(string inputString)
        {
            var regexItem = new Regex("^(?![0-9._])(?!.*[_]$)[a-zA-Z0-9_]+$");
            return regexItem.IsMatch(inputString);
        }
    }
}
