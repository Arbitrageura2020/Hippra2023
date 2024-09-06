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

        public async Task<IList<ProfileViewModel>> GetExpertsToFollow(string currentUserId)
        {
            using var _context = DbFactory.CreateDbContext();

            List<ProfileViewModel> users = await _context.Users.Where(x=>x.Id!=currentUserId).Select(x => new ProfileViewModel()
            {
               FirstName=x.FirstName,
               LastName=x.LastName,
                Userid = x.Id,
                MedicalSpecialty =x.MedicalSpecialty,
                ProfileUrl =x.ProfileUrl,
                Address =x.Address,
                ResidencyHospital =x.ResidencyHospital,
                Country =x.Country,
                NrOfFollowers=  _context.Follows.Count(c => c.FollowingUserID == x.Id)
            }).AsNoTracking().OrderByDescending(x=>x.NrOfFollowers).ToListAsync();

            return users;
        }
    }
}
