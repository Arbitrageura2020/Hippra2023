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
        // private readonly ApplicationDbContext _context;
        private AppSettings AppSettings { get; set; }

        private AzureStorage Storage;
        private ImageHelper ImageHelper;
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        public HttpContext WebContext => _httpContextAccessor.HttpContext;
        public HippraService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            IOptions<AppSettings> settings,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            AppSettings = settings?.Value;
            // _context = context;
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

            List<Case> cases = await _context.Cases.Where(x=>x.Type==type).Include(c => c.MedicalSubCategory).Include(c => c.Tags).Include(x=>x.User).OrderByDescending(s => s.DateCreated).AsNoTracking().ToListAsync();
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
        public async Task<Case> GetCaseNoTracking(int caseCaseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Include(x => x.MedicalSubCategory).Include(x => x.Tags).Include(x => x.Comments).AsNoTracking().FirstOrDefaultAsync(c => c.ID == caseCaseId);
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
        public async Task<bool> AddNewCase(AddEditCaseViewModel inputCase)
        {
            var userInfo = await _userManager.GetUserAsync(WebContext.User);

            using var _context = DbFactory.CreateDbContext();

            var newCase = new Case();

            newCase.UserId = userInfo!.Id;
            newCase.PosterName = userInfo.FirstName;
            newCase.PosterSpecialty = Enums.GetDisplayName(userInfo.MedicalSpecialty);
            newCase.Status = true;
            newCase.DateLastUpdated = DateTime.Now;
            newCase.DateCreated = DateTime.Now;
            newCase.PosterName = inputCase.PosterName;
            newCase.PosterSpecialty = inputCase.PosterSpecialty;
            newCase.Type = inputCase.Type;
            newCase.Topic = inputCase.Topic;
            newCase.Description = inputCase.Description;
            newCase.ResponseNeeded = inputCase.ResponseNeeded;
            newCase.MedicalCategory = inputCase.MedicalCategory;
            newCase.MedicalSubCategoryId = inputCase.MedicalSubCategoryId;
            newCase.PatientAge = inputCase.PatientAge;
      
            newCase.Gender = inputCase.Gender;
            newCase.Race = inputCase.Race;
            newCase.Ethnicity = inputCase.Ethnicity;
            newCase.LabValues = inputCase.LabValues;
            newCase.CurrentStageOfDisease = inputCase.CurrentStageOfDisease;
            newCase.imgUrl = inputCase.imgUrl;

            newCase.CurrentTreatmentAdministered = inputCase.CurrentTreatmentAdministered;
            newCase.TreatmentOutcomes = inputCase.TreatmentOutcomes;
            if (inputCase.Tags != null)
            {
                newCase.Tags = new List<CaseTags>();
                foreach (var tag in inputCase.Tags)
                {
                    newCase.Tags.Add(new CaseTags()
                    {
                        Tag = tag,
                        CaseID = newCase.ID,
                        ID = DateTime.Now
                    });
                }
            }

            try
            {
                await _context.AddAsync(newCase);
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
            caseObject.ResponseNeeded = inputCase.ResponseNeeded;
            caseObject.MedicalCategory = inputCase.MedicalCategory;
            caseObject.MedicalSubCategoryId = inputCase.MedicalSubCategoryId;
            caseObject.PatientAge = inputCase.PatientAge;

            caseObject.Gender = inputCase.Gender;
            caseObject.Race = inputCase.Race;
            caseObject.Ethnicity = inputCase.Ethnicity;
            caseObject.LabValues = inputCase.LabValues;
            caseObject.CurrentStageOfDisease = inputCase.CurrentStageOfDisease;

            caseObject.CurrentTreatmentAdministered = inputCase.CurrentTreatmentAdministered;
            caseObject.TreatmentOutcomes = inputCase.TreatmentOutcomes;
            caseObject.imgUrl = inputCase.imgUrl;
            if (inputCase.Tags != null && inputCase.Tags.Count > 0)
            {

                foreach (var tag in inputCase.Tags)
                {
                    if (!caseObject.Tags.Any(x => x.Tag == tag))
                    {
                        caseObject.Tags.Add(new CaseTags()
                        {
                            Tag = tag,
                            CaseID = caseObject.ID,
                            ID = DateTime.Now
                        });
                    }
                }
            }
            else
            {
                caseObject.Tags = new List<CaseTags>();
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
        public async Task<bool> AddTag(CaseTags Tag)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.CaseTags.Add(Tag);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<CaseTags>> GetTagsNoTracking(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.CaseTags.AsNoTracking().Where(c => c.CaseID == caseId).ToListAsync();
            return result;

        }
        public async Task<bool> DeleteTag(CaseTags tag)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.CaseTags.FirstOrDefaultAsync(t => t.ID == tag.ID && t.Tag == tag.Tag);

            if (CaseTag != null)
            {
                _context.CaseTags.Remove(CaseTag);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<int>> GetCasesIdByTag(string tag)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseTag = await _context.CaseTags.AsNoTracking().Where(t => t.Tag == tag).ToListAsync();
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
        // comments
        public async Task<List<CaseComment>> GetComments(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseComments.Where(c => c.CaseID == caseId).ToListAsync();
        }
        public async Task<List<CaseComment>> GetCommentsNoTracking(int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.CaseComments.Where(c => c.CaseID == caseId).Include(x => x.User).AsNoTracking().ToListAsync();
            return result;
        }
        public async Task<CaseComment> GetComment(int caseCommentId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseComments.FirstOrDefaultAsync(c => c.ID == caseCommentId);
        }
        public async Task<CaseComment> GetCommentNoTracking(int caseCommentId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseComments.AsNoTracking().FirstOrDefaultAsync(c => c.ID == caseCommentId);
        }

        [Authorize]
        public async Task<bool> AddComment(int caseId, string comment, string? img)
        {
            using var _context = DbFactory.CreateDbContext();
            var userInfo = await _userManager.GetUserAsync(WebContext.User);
            var addComment = new CaseComment();
            addComment.PosterId = userInfo.PublicId;
            addComment.PosterName = userInfo.FullName;
            addComment.PostedDate = DateTime.Now;
            addComment.LastUpdatedDate = DateTime.Now;
            addComment.posterSpeciality = Enums.GetDisplayName(userInfo.MedicalSpecialty);
            addComment.CaseID = caseId;
            addComment.Comment = comment;
            addComment.UserId = userInfo.Id;
            addComment.imgUrl = img;
            if (userInfo.ProfileUrl != null)
            {
                addComment.ProfileUrl = userInfo.ProfileUrl;
            }
            else
            {
                addComment.ProfileUrl = "/img/hippra/blank-profile.png";
            }
            _context.CaseComments.Add(addComment);
            await _context.SaveChangesAsync();


            return true;
        }
        public async Task<bool> EditComment(CaseComment EditedCaseComment, int type)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseComment = await _context.CaseComments.FirstOrDefaultAsync(m => m.ID == EditedCaseComment.ID);

            if (CaseComment == null)
            {
                return false;
            }
            CaseComment.Comment = EditedCaseComment.Comment;
            CaseComment.imgUrl = EditedCaseComment.imgUrl;
            CaseComment.ProfileUrl = EditedCaseComment.ProfileUrl;
            if (type == -2)
            {
                CaseComment.LastUpdatedDate = DateTime.Now;
            }
            else if (type == -3)
            {
                CaseComment.VoteUp = EditedCaseComment.VoteUp;
            }
            else
            {
                CaseComment.VoteDown = EditedCaseComment.VoteDown;
            }
            try
            {
                _context.Update(CaseComment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseCommentExists(EditedCaseComment.ID))
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
        private bool CaseCommentExists(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.CaseComments.AsNoTracking().Any(e => e.ID == id);
        }

        public async Task<bool> DeleteComment(int caseCommentId)
        {
            using var _context = DbFactory.CreateDbContext();

            var CaseComment = await _context.CaseComments.FindAsync(caseCommentId);

            if (CaseComment != null)
            {
                _context.CaseComments.Remove(CaseComment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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
        public async Task<PostHistory> GetHistoryByIDs(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            PostHistory h = await _context.PostHistories.FirstOrDefaultAsync(h => h.ID == id);
            return h;
        }
        //Vote
        public async Task<bool> AddVote(Vote newVote)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Votes.Add(newVote);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CheckVoter(int postId, int voterId, int cID)
        {
            using var _context = DbFactory.CreateDbContext();

            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.PosterID == postId && v.VoterID == voterId && v.CID == cID);
            if (vote != null)
            {
                return true;
            }
            return false;
        }
        //Stats
        public async Task<Stats> GetStats(int postId)
        {
            using var _context = DbFactory.CreateDbContext();

            Stats stats = new Stats();
            stats.UpVote = await _context.Votes.AsNoTracking().CountAsync(v => v.PosterID == postId);
            stats.Votes = await _context.Votes.AsNoTracking().CountAsync(v => v.VoterID == postId);
            stats.Answers = await _context.CaseComments.AsNoTracking().CountAsync(c => c.PosterId == postId && c.PosterId != c.Case.PosterID);
            return stats;
        }
        public async Task<int> GetCommentCount(int posterId)
        {
            using var _context = DbFactory.CreateDbContext();
            return await _context.CaseComments.AsNoTracking().CountAsync(c => c.PosterId == posterId);
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
        //Notification
        public async Task<bool> AddNotification(Notification notif)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<NotificationResultModel> GetAllNotifications(int userID, int targetPage, int PageSize)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Notification> ListNotifs = await _context.Notifications.Where(n => n.ReceiverID == userID).OrderByDescending(n => n.CreationDate).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            NotificationResultModel result = new NotificationResultModel();
            result.Notifications = ListNotifs;
            result.TotalCount = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverID == userID);
            return result;
        }
        public async Task<int> CountMyNotification(int userID)
        {
            using var _context = DbFactory.CreateDbContext();

            int count = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverID == userID && s.IsRead == -1);
            return count;
        }
        public async Task<bool> DeleteNotification(int nID)
        {
            using var _context = DbFactory.CreateDbContext();

            Notification notif = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationID == nID);
            if (notif != null)
            {
                _context.Notifications.Remove(notif);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> NotificationRead(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.ID == id);

            if (notif == null)
            {
                return false;
            }

            notif.IsRead = 2;
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
        private bool NotificationExists(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Notifications.AsNoTracking().Any(n => n.ID == id);
        }
        //Follow
        public async Task<bool> AddFollower(Follow newFollower)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.Follows.Add(newFollower);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveFollower(int follower, int following)
        {
            using var _context = DbFactory.CreateDbContext();

            Follow f = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerUserID == follower && f.FollowingUserID == following);
            if (f != null)
            {
                _context.Follows.Remove(f);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<Follow>> GetAllFollowers(int my_Id)
        {
            using var _context = DbFactory.CreateDbContext();

            List<Follow> followers = await _context.Follows.Where(c => c.FollowingUserID == my_Id).ToListAsync();
            return followers;
        }
        public async Task<bool> CheckFollower(int my_Id, int f_Id)
        {
            using var _context = DbFactory.CreateDbContext();

            var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerUserID == my_Id && f.FollowingUserID == f_Id);
            if (follow != null)
            {
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
