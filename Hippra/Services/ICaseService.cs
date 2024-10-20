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
using Hippra.Models.DTO;

namespace Hippra.Services
{
    public interface ICaseService
    {
        Task<IList<CaseViewModel>> GetMyCases(string userId);
        Task<CaseViewModel> GetCaseNoTracking(long caseId, string currentUserId);
        Task<Result> AddNewCase(AddEditCaseViewModel inputCase);
        Task<Result> EditCase(AddEditCaseViewModel inputCase);
        Task<AddEditCaseViewModel> GetCase(long caseId);
        Task<bool> CheckLike(string userId, long caseId);
        Task<bool> AddLike(string userId, long caseId);
        Task<bool> RemoveLike(string userId, long caseId);
        Task<Result> SaveCaseFile(Stream fileStream, long caseId, string fileName, string fileType, string userId);
        Task<FileDownloadResult> DownloadCaseFile(long id);
        Task<IList<CaseViewModel>> GetExploreTopics(List<int> selectedTags);
        Task<IList<CaseViewModel>> GetTrendingPosts();

        //comments
        Task<List<CaseCommentViewModel>> GetCommentsNoTracking(long caseId, string currentUserId);
        Task<int> GetCommentAddedCount(string posterId);
        Task<CaseCommentViewModel> GetComment(long commentId, string currentUserId);
        Task<Result> AddComment(long caseId, string comment, string userId);
        Task<Result> UpdateComment(long commentId, string comment);
        Task<Result> SaveCaseCommentFile(Stream fileStream, long caseCommentId, string fileName, string fileType, string userId);
        Task<Result> DeleteCommentFile(long commentFileId, string userId);
        Task<FileDownloadResult> DownloadCaseCommentFile(long id);
        Task<bool> DeleteComment(long commentId, string userId);

        Task<bool> CheckVote(string voterId, long commentId);
        Task<bool> AddVote(string voterId, long commentId);
        Task<bool> RemoveVote(string voterId, long commentId);
        Task<bool> ReportComment(string userId, long commentId, long caseId, string reportText);
    }
}
