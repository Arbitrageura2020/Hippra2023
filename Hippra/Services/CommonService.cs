﻿using Hippra.Data;
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


        public async Task AddHistoryPost(Case pCase)
        {
            var userInfo = await _userManager.GetUserAsync(WebContext.User);
            PostHistory newHistory = new PostHistory();
            List<Follow> followerList = new List<Follow>();
            int lastHistoryId = -1;
            //history stuffs
            newHistory.ID = 0;
            newHistory.PostID = pCase.ID;
            newHistory.CreationDate = DateTime.Now;
            newHistory.PosterID = userInfo.PublicId;
            newHistory.UserDisplayName = userInfo.FirstName;
            newHistory.Title = pCase.Topic;
            newHistory.Detail = pCase.Description;
            newHistory.HistoryTypes = "added a new case";
            lastHistoryId = await hService.AddHistory(newHistory);
            //follow
            followerList = await hService.GetAllFollowers(userInfo.PublicId);
            foreach (var follower in followerList)
            {
                Notification newNotifs = new Notification();
                newNotifs.ID = 0;
                newNotifs.SenderID = follower.FollowingUserID;
                newNotifs.ReceiverID = follower.FollowerUserID;
                newNotifs.NotificationID = lastHistoryId;
                newNotifs.IsRead = -1;
                newNotifs.IsResponseNeeded = -1;
                newNotifs.CreationDate = DateTime.Now;
                await hService.AddNotification(newNotifs);
            }
        }
    }
}
