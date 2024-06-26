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
    public interface ICaseService
    {
        Task<Case> GetCaseNoTracking(int caseId);

        //comments
        Task<List<CaseCommentViewModel>> GetCommentsNoTracking(int caseId);
        Task<int> GetCommentAddedCount(string posterId);
        Task<Result> AddComment(int caseId, string comment, string userId);
        Task<Result> UpdateComment(long commentId, string comment);
        Task<bool> CheckVote(string voterId, long commentId);
        Task<Result> SaveCaseCommentFile(Stream fileStream, long caseCommentId, string fileName, string fileType, string userId);
    }
}
