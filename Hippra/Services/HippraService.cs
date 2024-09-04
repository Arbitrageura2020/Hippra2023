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

        public async Task<IList<CaseViewModel>> GetAllCases(CaseType type)
        {
            using var _context = DbFactory.CreateDbContext();

            List<CaseViewModel> cases = await _context.Cases.Where(x => x.Type == type).Include(c => c.Tags).Include(x => x.User).OrderByDescending(s => s.DateCreated).Select(x => new CaseViewModel()
            {
                ID = x.ID,
                Description = x.Description,
                Topic = x.Topic,
                DateLastUpdated = x.DateLastUpdated,
                PosterName = x.User.FullName,
                PosterId = x.User.Id,
                PosterSpeciality = x.User.MedicalSpecialty != null ? @EnumsHelper.GetDisplayName(x.User.MedicalSpecialty) : "",
                PosterImg = x.User.ProfileUrl,
                CurrentStageOfDisease = x.CurrentStageOfDisease,
                CurrentTreatmentAdministered = x.CurrentTreatmentAdministered,
                DateCreated = x.DateCreated,
                LabValues = x.LabValues,
                Ethnicity = x.Ethnicity,
                Gender = x.Gender,
                Type = x.Type,
                Race = x.Race,
                PatientAge = x.PatientAge,
                TreatmentOutcomes = x.TreatmentOutcomes,
                Status = x.Status,
                Tags = x.Tags.Select(t => new CaseTagViewModel()
                {
                    Name = t.Name,
                    TagId = t.ID,
                }).ToList(),
                User=x.User,
            }).AsNoTracking().ToListAsync();

            return cases;
        }

        public async Task<SearchResultModel> GetMyCases(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Case> cases = await _context.Cases.Where(x => x.UserId == userId).Include(c => c.MedicalSubCategory).Include(c => c.Tags).Include(x => x.User).OrderByDescending(s => s.DateCreated).AsNoTracking().ToListAsync();
            SearchResultModel result = new SearchResultModel();
            result.TotalCount = cases.Count;
            result.Cases = cases;
            return result;
        }

        public async Task<List<Case>> GetCases(int CurrentPage, int PageSize, int id)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Case> cases = null;
            if (id == -1)
            {
                cases = await _context.Cases.OrderByDescending(s => s.DateCreated).Include(c => c.Comments).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
            }
            else
            {
                cases = await _context.Cases.Where(u => u.PosterID == id).OrderByDescending(s => s.DateCreated).Include(c => c.Comments).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
            }
            return cases;
        }
        public async Task<SearchResultModel> GetCasesNoTracking(string searchString, bool showClosed, bool showTagOnly, int SubCategory, int Priority, int CurrentPage, int PageSize, int id, List<int> caseIDs)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Case> cases = new List<Case>();
            Case tempCase = new Case();
            int count = 0;
            //cases = await _context.Cases.ToListAsync();
            int begin = (CurrentPage - 1) * PageSize;
            int end = begin + PageSize;

            SearchResultModel result = new SearchResultModel();
            result.TotalCount = count;
            result.Cases = cases;

            return result;
        }

        public async Task<Case> GetCase(int CaseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.FirstOrDefaultAsync(c => c.ID == CaseId);
            return result;
        }
        public async Task<Case> GetCaseNoTracking(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Where(x => x.ID == caseId).Include(x => x.User).Include(c => c.Tags).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> AddCase(Case Case)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Cases.Add(Case);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> CreateEmptyCase(int userId)
        {
            using var _context = DbFactory.CreateDbContext();

            string key = $"{Guid.NewGuid().ToString()}";
            int id = -1;

            Case c = new Case();
            c.PosterID = userId;
            c.Description = key;
            _context.Cases.Add(c);
            _context.SaveChanges();

            c = await _context.Cases.FirstOrDefaultAsync(s => s.Description == key);
            if (c != null)
            {
                id = c.ID;
                c.Description = "";
                _context.Cases.Update(c);
                await _context.SaveChangesAsync();
            }
            return id;

        }





        [Authorize]
        public async Task<bool> EditCase(AddEditCaseViewModel inputCase)
        {
            var userInfo = await _userManager.GetUserAsync(WebContext.User);
            using var _context = DbFactory.CreateDbContext();

            var caseObject = await _context.Cases.Include(x => x.Tags).FirstOrDefaultAsync(m => m.ID == inputCase.ID);

            if (caseObject == null)
            {
                return false;
            }
            if (!inputCase.Status)
            {
                caseObject.DateClosed = DateTime.Now;
                caseObject.Status = false;
            }

            caseObject.DateLastUpdated = DateTime.Now;

            caseObject.Topic = inputCase.Topic;
            caseObject.Description = inputCase.Description;
            caseObject.PatientAge = inputCase.PatientAge;

            caseObject.Gender = inputCase.Gender;
            caseObject.Race = inputCase.Race;
            caseObject.Ethnicity = inputCase.Ethnicity;
            caseObject.LabValues = inputCase.LabValues;
            caseObject.CurrentStageOfDisease = inputCase.CurrentStageOfDisease;

            caseObject.CurrentTreatmentAdministered = inputCase.CurrentTreatmentAdministered;
            caseObject.TreatmentOutcomes = inputCase.TreatmentOutcomes;
            caseObject.imgUrl = inputCase.imgUrl;

            if (inputCase.SelectedTagsObjects != null && inputCase.SelectedTagsObjects.Any())
            {
                caseObject.Tags = inputCase.SelectedTagsObjects.ToList();
            }

            try
            {
                _context.Update(caseObject);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(inputCase.ID))
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

        public async Task<bool> CloseCase(int CaseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var Case = await _context.Cases.FirstOrDefaultAsync(m => m.ID == CaseId);

            if (Case == null)
            {
                return false;
            }
            Case.DateClosed = DateTime.Now;
            Case.Status = !Case.Status; // closed

            try
            {
                _context.Update(Case);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(CaseId))
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
        private bool CaseExists(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Any(e => e.ID == id);
        }
        public async Task<bool> DeleteCase(int caseCaseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var Case = await _context.Cases.FindAsync(caseCaseId);

            if (Case != null)
            {
                _context.Cases.Remove(Case);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Tags

        public async Task<List<Tag>> GetTagsNoTracking(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.PostTags.Include(x => x.Tag).Select(x => x.Tag).ToListAsync();
            return result;

        }

        public async Task<List<int>> GetCasesIdByTag(int tagId)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.PostTags.AsNoTracking().Where(t => t.TagsId == tagId).ToListAsync();
            List<int> result = new List<int>();
            if (CaseTag != null)
            {

                foreach (var item in CaseTag)
                {
                    result.Add(item.CasesID);
                }
            }

            return result;
        }

        public async Task<List<Case>> GetCasesByTag(int tagId)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.PostTags.AsNoTracking().Where(t => t.TagsId == tagId).Include(x => x.Case).ThenInclude(x => x.User).ToListAsync();
            List<Case> result = new List<Case>();
            if (CaseTag != null)
            {

                foreach (var item in CaseTag)
                {
                    result.Add(item.Case);
                }
            }

            return result;
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
