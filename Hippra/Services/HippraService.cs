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

        public async Task<SearchResultModel> GetAllCases(CaseType type)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Case> cases = await _context.Cases.Where(x => x.Type == type).Include(c => c.MedicalSubCategory).Include(c => c.Tags).Include(x => x.User).OrderByDescending(s => s.DateCreated).AsNoTracking().ToListAsync();
            SearchResultModel result = new SearchResultModel();
            result.TotalCount = cases.Count;
            result.Cases = cases;
            return result;
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

            if (SubCategory == -1)
            {
                if (Priority == -1)
                {
                    if (showClosed)
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (!cases.Contains(tempCase))
                                        {
                                            cases.Add(tempCase);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }

                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.Status && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (showClosed)
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == (CaseResponseLevelType)Priority)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == (CaseResponseLevelType)Priority && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == (CaseResponseLevelType)Priority && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == (CaseResponseLevelType)Priority && tempCase.Status && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.Status && u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (Priority == -1)
                {
                    if (showClosed)
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.PosterID == id && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (showClosed)
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.ResponseNeeded == (CaseResponseLevelType)Priority)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.ResponseNeeded == (CaseResponseLevelType)Priority && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id && u.ResponseNeeded == (CaseResponseLevelType)Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (showTagOnly)
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalSubCategoryId == SubCategory && tempCase.Status && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }

                                    //cases = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.Include(x=>x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                                cases.OrderByDescending(s => s.DateCreated);
                                count = cases.Count;
                                if (count > end)
                                {
                                    cases = cases.GetRange(begin, end);
                                }
                                else
                                {
                                    cases = cases.GetRange(begin, count);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(s => s.MedicalSubCategoryId == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Where(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().CountAsync(u => u.MedicalSubCategoryId == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                    }
                }
            }

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

            var result = await _context.Cases.Where(x => x.ID == caseId).Include(c => c.MedicalSubCategory).Include(c => c.Tags).Include(x => x.User).FirstOrDefaultAsync();
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

            // caseObject.UserId = userInfo!.Id;
            //caseObject.PosterName = userInfo.FirstName;
            //caseObject.PosterSpecialty = Enums.GetDisplayName(userInfo.MedicalSpecialty);
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
            caseObject.Tags = new List<PostTags>();
            if (inputCase.SelectedTags != null && inputCase.SelectedTags.Count() > 0)
            {
                foreach (var tagId in inputCase.SelectedTags)
                {
                    caseObject.Tags.Add(new PostTags()
                    {
                        TagId = tagId,
                        CaseID = caseObject.ID,
                    });
                }
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
        public async Task<bool> AddTag(PostTags Tag)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.PostTags.Add(Tag);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<PostTags>> GetTagsNoTracking(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.PostTags.Include(x=>x.Tag).AsNoTracking().Where(c => c.CaseID == caseId).ToListAsync();
            return result;

        }
        public async Task<bool> DeleteTag(PostTags tag)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.PostTags.FirstOrDefaultAsync(t => t.ID == tag.ID);

            if (CaseTag != null)
            {
                _context.PostTags.Remove(CaseTag);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<int>> GetCasesIdByTag(int tagId)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.PostTags.AsNoTracking().Where(t => t.TagId == tagId).ToListAsync();
            List<int> result = new List<int>();
            if (CaseTag != null)
            {

                foreach (var item in CaseTag)
                {
                    result.Add(item.CaseID);
                }
            }

            return result;
        }

        public async Task<List<Case>> GetCasesByTag(int tagId)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.PostTags.AsNoTracking().Where(t => t.TagId == tagId).Include(x => x.Case).ThenInclude(x => x.User).ToListAsync();
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
