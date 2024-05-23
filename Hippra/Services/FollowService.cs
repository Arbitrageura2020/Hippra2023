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
using Microsoft.AspNetCore.Authorization;
using Hippra.Models.ViewModel;
using Hippra.Components;

namespace Hippra.Services
{
    public class FollowService : IFollowService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AppSettings AppSettings { get; set; }

        private AzureStorage Storage;
        private ImageHelper ImageHelper;
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        public HttpContext WebContext => _httpContextAccessor.HttpContext;
        public FollowService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOptions<AppSettings> settings,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            AppSettings = settings?.Value;
            Storage = new AzureStorage(settings);
            ImageHelper = new ImageHelper(Storage);
            DbFactory = dbFactory;
            _httpContextAccessor = httpContextAccessor;
        }


        //Connections
        public async Task<bool> AddConnection(Connection conn)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Connections.Add(conn);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangeConnectionStatus(int userId, int friendId)
        {
            using var _context = DbFactory.CreateDbContext();

            var conn = await _context.Connections.FirstOrDefaultAsync(c => c.UserID == userId && c.FriendID == friendId);

            if (conn == null)
            {
                return false;
            }

            conn.Status = 1;
            try
            {
                _context.Update(conn);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConnectionExists(conn.ID))
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
        private bool ConnectionExists(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Connections.AsNoTracking().Any(n => n.ID == id);
        }
        public async Task<string> CheckConnection(int my_Id, int f_Id)
        {
            using var _context = DbFactory.CreateDbContext();

            var conn = await _context.Connections.FirstOrDefaultAsync(c => (c.UserID == my_Id && c.FriendID == f_Id) || (c.UserID == f_Id && c.FriendID == my_Id));
            if (conn != null)
            {
                if (conn.Status == -1)
                {
                    return "P";
                }
                else
                {
                    return "C";
                }
            }
            return "NC";
        }
        public async Task<ConnResultModel> GetAllConnections(int my_Id, int targetPage, int PageSize)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Connection> conn = await _context.Connections.Where(c => (c.UserID == my_Id || c.FriendID == my_Id) && c.Status == 1).OrderByDescending(s => s.ID).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync(); ;
            ConnResultModel result = new ConnResultModel();
            result.Connections = conn;
            result.TotalCount = await _context.Connections.AsNoTracking().CountAsync(c => (c.UserID == my_Id || c.FriendID == my_Id) && c.Status == 1);
            return result;
        }

        public async Task<bool> RemoveConnection(int userId, int fID)
        {
            using var _context = DbFactory.CreateDbContext();

            Connection conn = await _context.Connections.FirstOrDefaultAsync(c => c.UserID == userId && c.FriendID == fID);
            if (conn != null)
            {
                _context.Connections.Remove(conn);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }




        //Follow
        public async Task<bool> AddFollower(string followingId, string followerId)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Follows.Add(new Follow() { FollowerUserID = followerId, FollowingUserID = followingId });
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveFollower(string followingId, string followerId )
        {
            using var _context = DbFactory.CreateDbContext();

            Follow f = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerUserID == followerId && f.FollowingUserID == followingId);
            if (f != null)
            {
                _context.Follows.Remove(f);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Follow>> GetAllFollowers(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Follow> followers = await _context.Follows.Where(c => c.FollowingUserID == userId).ToListAsync();
            return followers;
        }

        public async Task<bool> CheckFollower(string myId, string followingId)
        {
            using var _context = DbFactory.CreateDbContext();

            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerUserID == myId && f.FollowingUserID == followingId);
            if (follow != null)
            {
                return true;
            }
            return false;
        }

        public async Task<int> GetNrOfFollowers(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var followers = await _context.Follows.CountAsync(c => c.FollowingUserID == userId);
            return followers;
        }

        public async Task<int> GetNrOfFollowing(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var followers = await _context.Follows.CountAsync(c => c.FollowerUserID == userId);
            return followers;
        }
    }
}
