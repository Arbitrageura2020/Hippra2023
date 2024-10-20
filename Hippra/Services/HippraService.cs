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
    public class HippraService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AppSettings AppSettings { get; set; }

        private AzureStorage Storage;
        private ImageHelper ImageHelper;
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        public HttpContext WebContext => _httpContextAccessor.HttpContext;
        public HippraService(
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

        public bool AddMedicalCategory(IList<MedicalSubCategory> items)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.MedicalSubCategories.AddRange(items);
            _context.SaveChanges();
            return true;
        }

        public async Task<int> GetCaseCount()
        {

            using var _context = DbFactory.CreateDbContext();

            return _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Count();
        }
        public async Task<int> GetMyCaseCount(int profileId)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.PosterID == profileId).Count();
        }

        //Stats
        //public async Task<Stats> GetStats(int postId)
        //{
        //    using var _context = DbFactory.CreateDbContext();

        //    Stats stats = new Stats();
        //    stats.UpVote = await _context.CaseCommentVotes.AsNoTracking().CountAsync(v => v.PosterID == postId);
        //    stats.Votes = await _context.CaseCommentVotes.AsNoTracking().CountAsync(v => v.VoterID == postId);
        //    stats.Answers = await _context.CaseComments.AsNoTracking().CountAsync(c => c.PosterId == postId && c.PosterId != c.Case.PosterID);
        //    return stats;
        //}


        // image upload
        public async Task<string> UploadImgToAzureAsync(Stream fileStream, string fileName)
        {
            return await ImageHelper.UploadImageToStorage(fileStream, fileName);
        }

        public async Task<bool> DeleteImage(string filename)
        {
            await ImageHelper.DeleteImageToStorage(filename);
            return true;
        }
        public string GetImgStorageUrl()
        {
            return AppSettings.StorageUrl;
        }
        public async Task<bool> ImageUploadFunc(Stream file, string name)
        {
            string filename = await ImageHelper.UploadImageToStorage(file, name).ConfigureAwait(true);
            return true;
        }


    }
}
