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

namespace Hippra.Services
{
    public class HippraService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly ApplicationDbContext _context;
        private AppSettings AppSettings { get; set; }

        private AzureStorage Storage;
        private ImageHelper ImageHelper;
        public HippraService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            IOptions<AppSettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            AppSettings = settings?.Value;
            _context = context;
            Storage = new AzureStorage(settings);
            ImageHelper = new ImageHelper(Storage);
        }

        public async Task<int> GetCaseCount()
        {
            return _context.Cases.AsNoTracking().Count();
        }
        public async Task<int> GetMyCaseCount(int profileId)
        {
            return _context.Cases.AsNoTracking().Where(s=>s.PosterID == profileId).Count();
        }
        public async Task<List<Case>> GetCases(int CurrentPage, int PageSize, int id)
        {
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
                                    foreach(var Id in caseIDs)
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
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.PosterID == id);
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
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
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
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
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
                                        if (tempCase.ResponseNeeded == Priority)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == Priority && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.PosterID == id);
                                }
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync();
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.PosterID == id);
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
                                        if (tempCase.ResponseNeeded == Priority && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.ResponseNeeded == Priority && tempCase.Status && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.Status && u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.Status && u.PosterID == id);
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
                                        if (tempCase.MedicalCategory == SubCategory)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString)).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.PosterID == id);
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
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.PosterID == id && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id);
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
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.ResponseNeeded == Priority)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.ResponseNeeded == Priority && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString));
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.PosterID == id && u.ResponseNeeded == Priority).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.PosterID == id);
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
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.Status)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    foreach (var Id in caseIDs)
                                    {
                                        tempCase = await GetCase(Id);
                                        if (tempCase.MedicalCategory == SubCategory && tempCase.Status && tempCase.PosterID == id)
                                        {
                                            if (!cases.Contains(tempCase))
                                            {
                                                cases.Add(tempCase);
                                            }
                                        }
                                    }
                                    
                                    //cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    //count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
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
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(s => s.MedicalCategory == SubCategory && s.Topic.Contains(searchString) && s.Status && s.PosterID == id);
                                }
                            }
                            else
                            {
                                if (id == -1)
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status);
                                }
                                else
                                {
                                    cases = await _context.Cases.AsNoTracking().Where(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id).OrderByDescending(s => s.DateCreated).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
                                    count = await _context.Cases.AsNoTracking().CountAsync(u => u.MedicalCategory == SubCategory && u.Status && u.PosterID == id);
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
            var result = await _context.Cases.FirstOrDefaultAsync(c => c.ID == CaseId);
            return result;
        }
        public async Task<Case> GetCaseNoTracking(int caseCaseId)
        {
            var result = await _context.Cases.AsNoTracking().FirstOrDefaultAsync(c => c.ID == caseCaseId);
            return result;
        }
        public async Task<bool> AddCase(Case Case)
        {
            _context.Cases.Add(Case);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> CreateEmptyCase(int userId)
        {
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


        public async Task<bool> CreateCase(Case EditedCase)
        {
            var Case = await _context.Cases.FirstOrDefaultAsync(m => m.ID == EditedCase.ID);

            if (Case == null)
            {
                return false;
            }
            Case.Status = true;
            Case.DateLastUpdated = DateTime.Now;
            Case.DateCreated = EditedCase.DateCreated;
            Case.PosterName = EditedCase.PosterName;
            Case.PosterSpecialty = EditedCase.PosterSpecialty;

            Case.Topic = EditedCase.Topic;
            Case.Description = EditedCase.Description;
            Case.ResponseNeeded = EditedCase.ResponseNeeded;
            Case.MedicalCategory = EditedCase.MedicalCategory;
            Case.PatientAge = EditedCase.PatientAge;

            Case.Gender = EditedCase.Gender;
            Case.Race = EditedCase.Race;
            Case.Ethnicity = EditedCase.Ethnicity;
            Case.LabValues = EditedCase.LabValues;
            Case.CurrentStageOfDisease = EditedCase.CurrentStageOfDisease;
            Case.imgUrl = EditedCase.imgUrl;

            Case.CurrentTreatmentAdministered = EditedCase.CurrentTreatmentAdministered;
            Case.TreatmentOutcomes = EditedCase.TreatmentOutcomes;

            try
            {
                _context.Update(Case);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(EditedCase.ID))
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

        public async Task<bool> EditCase(Case EditedCase)
        {
            var Case = await _context.Cases.FirstOrDefaultAsync(m => m.ID == EditedCase.ID);

            if (Case == null)
            {
                return false;
            }
            if (!EditedCase.Status)
            {
                Case.DateClosed = DateTime.Now;
                Case.Status = false;
            }
           
            Case.DateLastUpdated = DateTime.Now;

            Case.Topic = EditedCase.Topic;
            Case.Description = EditedCase.Description;
            Case.ResponseNeeded = EditedCase.ResponseNeeded;
            Case.MedicalCategory = EditedCase.MedicalCategory;
            Case.PatientAge = EditedCase.PatientAge;

            Case.Gender = EditedCase.Gender;
            Case.Race = EditedCase.Race;
            Case.Ethnicity = EditedCase.Ethnicity;
            Case.LabValues = EditedCase.LabValues;
            Case.CurrentStageOfDisease = EditedCase.CurrentStageOfDisease;

            Case.CurrentTreatmentAdministered = EditedCase.CurrentTreatmentAdministered;
            Case.TreatmentOutcomes = EditedCase.TreatmentOutcomes;
            Case.imgUrl = EditedCase.imgUrl;
            try
            {
                _context.Update(Case);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(EditedCase.ID))
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
            return _context.Cases.AsNoTracking().Any(e => e.ID == id);
        }
        public async Task<bool> DeleteCase(int caseCaseId)
        {
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
            _context.CaseTags.Add(Tag);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<CaseTags>> GetTagsNoTracking(int caseId)
        {
            var result = await _context.CaseTags.AsNoTracking().Where(c => c.CaseID == caseId).ToListAsync();
            return result;

        }
        public async Task<bool> DeleteTag(CaseTags tag)
        {
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
            return await _context.CaseComments.Where(c => c.CaseID== caseId).ToListAsync();
        }
        public async Task<List<CaseComment>> GetCommentsNoTracking(int caseId)
        {
            var result = await _context.CaseComments.AsNoTracking().Where(c => c.CaseID == caseId).ToListAsync();
            return result;
        }
        public async Task<CaseComment> GetComment(int caseCommentId)
        {
            return await _context.CaseComments.FirstOrDefaultAsync(c => c.ID == caseCommentId);
        }
        public async Task<CaseComment> GetCommentNoTracking(int caseCommentId)
        {
            return await _context.CaseComments.AsNoTracking().FirstOrDefaultAsync(c => c.ID == caseCommentId);
        }
        public async Task<bool> AddComment(CaseComment CaseComment)
        {
            if (CaseComment != null)
            {
                CaseComment.LastUpdatedDate = DateTime.Now;
                CaseComment.PostedDate = CaseComment.LastUpdatedDate;
                _context.CaseComments.Add(CaseComment);
                await _context.SaveChangesAsync();
            }
           
            return true;
        }
        public async Task<bool> EditComment(CaseComment EditedCaseComment,int type)
        {
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
            else if(type == -3)
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
            return _context.CaseComments.AsNoTracking().Any(e => e.ID == id);
        }

        public async Task<bool> DeleteComment(int caseCommentId)
        {
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
            _context.PostHistories.Add(newHistory);
            await _context.SaveChangesAsync();
            return newHistory.ID;
        }
        public async Task<HistoryResultModel> GetPostHistories(int posterID, int targetPage, int PageSize)
        {
            List<PostHistory> histories = await _context.PostHistories.Where(c => c.PosterID == posterID).OrderByDescending(s => s.CreationDate).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            HistoryResultModel result = new HistoryResultModel();
            result.Histories = histories;
            result.TotalCount = await _context.PostHistories.AsNoTracking().CountAsync(s => s.PosterID == posterID);
            return result;
        }
        public async Task<PostHistory> GetHistoryByIDs(int id)
        {
            PostHistory h = await _context.PostHistories.FirstOrDefaultAsync(h => h.ID == id);
            return h;
        }
        //Vote
        public async Task<bool> AddVote(Vote newVote)
        {
            _context.Votes.Add(newVote);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CheckVoter(int postId, int voterId, int cID)
        {
            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.PosterID == postId && v.VoterID == voterId && v.CID == cID);
            if(vote != null)
            {
                return true;
            }
            return false;
        }
        //Stats
        public async Task<Stats> GetStats(int postId)
        {
            Stats stats = new Stats();
            stats.UpVote = await _context.Votes.AsNoTracking().CountAsync(v => v.PosterID == postId);
            stats.Votes = await _context.Votes.AsNoTracking().CountAsync(v => v.VoterID == postId);
            stats.Answers = await _context.CaseComments.AsNoTracking().CountAsync(c => c.PosterId == postId && c.PosterId != c.Case.PosterID);
            return stats;
        }
        //Connections
        public async Task<bool> AddConnection(Connection conn)
        {
            _context.Connections.Add(conn);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangeConnectionStatus(int userId, int friendId)
        {
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
            return _context.Connections.AsNoTracking().Any(n => n.ID == id);
        }
        public async Task<string> CheckConnection(int my_Id, int f_Id)
        {
            var conn = await _context.Connections.FirstOrDefaultAsync(c => (c.UserID == my_Id && c.FriendID == f_Id) || (c.UserID == f_Id && c.FriendID == my_Id));
            if(conn != null)
            {
                if(conn.Status == -1)
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
            List<Connection> conn = await _context.Connections.Where(c => (c.UserID == my_Id || c.FriendID == my_Id) && c.Status == 1).OrderByDescending(s => s.ID).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync(); ;
            ConnResultModel result = new ConnResultModel();
            result.Connections = conn;
            result.TotalCount = await _context.Connections.AsNoTracking().CountAsync(c => (c.UserID == my_Id || c.FriendID == my_Id) && c.Status == 1);
            return result;
        }

        public async Task<bool> RemoveConnection(int userId, int fID)
        {
            Connection conn = await _context.Connections.FirstOrDefaultAsync(c => c.UserID == userId && c.FriendID == fID);
            if(conn != null)
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
            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<NotificationResultModel> GetAllNotifications(int userID, int targetPage, int PageSize)
        {
            List<Notification> ListNotifs = await _context.Notifications.Where(n => n.ReceiverID == userID).OrderByDescending(n => n.CreationDate).Skip((targetPage - 1) * PageSize).Take(PageSize).ToListAsync();
            //var h = histories.OrderByDescending(h => h.CreationDate);
            NotificationResultModel result = new NotificationResultModel();
            result.Notifications = ListNotifs;
            result.TotalCount = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverID == userID);
            return result;
        }
        public async Task<int> CountMyNotification(int userID)
        {
            int count = await _context.Notifications.AsNoTracking().CountAsync(s => s.ReceiverID == userID && s.IsRead == -1);
            return count;
        }
        public async Task<bool> DeleteNotification(int nID)
        {
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
            return _context.Notifications.AsNoTracking().Any(n => n.ID == id);
        }
        //Follow
        public async Task<bool> AddFollower(Follow newFollower)
        {
            _context.Follows.Add(newFollower);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveFollower(int follower, int following)
        {
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
            List<Follow> followers = await _context.Follows.Where(c => c.FollowingUserID == my_Id).ToListAsync();
            return followers;
        }
        public async Task<bool> CheckFollower(int my_Id, int f_Id)
        {
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
