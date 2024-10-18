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
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.CodeAnalysis.Operations;
using SendGrid.Helpers.Mail;
using System.CodeDom;
using Hippra.Components.Pages;

namespace Hippra.Services
{
    public class CaseService : ICaseService
    {
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        private AzureStorage _azureStorage;
        private IFileClient _fileClient;
        private AppSettings AppSettings { get; set; }
        public CaseService(
            IDbContextFactory<ApplicationDbContext> dbFactory, AzureStorage azureStorage, IOptions<AppSettings> settings, IFileClient fileClient)
        {
            DbFactory = dbFactory;
            _azureStorage = azureStorage;
            AppSettings = settings?.Value;
            _fileClient = fileClient;
        }

        public async Task<IList<CaseViewModel>> GetMyCases(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Where(x => x.UserId == userId && !x.PostedAnonymosley).Select(x => new CaseViewModel()
            {
                ID = x.ID,
                Description = x.Description,
                Topic = x.Topic,
                DateLastUpdated = x.DateLastUpdated,
                PosterName = x.User.FullName,
                PosterId = x.User.Id,
                PosterSpeciality = @EnumsHelper.GetDisplayName(x.User.MedicalSpecialty),
                PosterImg = x.User.ProfileUrl
             /*   IsAnonymos = x.IsAnonymus*/,
                //ImOwner = x.UserId == currentUserId
                Tags = x.Tags.Select(t => new CaseTagViewModel()
                {
                    Name = t.Name,
                    TagId = t.ID,
                }).ToList()
            }).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<AddEditCaseViewModel> GetCase(long caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Where(x => x.ID == caseId).Select(x => new AddEditCaseViewModel()
            {
                ID = x.ID,
                Description = x.Description,
                Topic = x.Topic,
                DateLastUpdated = x.DateLastUpdated,
                PosterName = x.User.FullName,
                UserId = x.User.Id,
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
                SelectedTagsObjects = x.Tags,
                Files = x.Files.Select(t => new CaseFileViewModel()
                {
                    ID = t.ID,
                    FileName = t.FileName,
                    FileType = t.FileType,
                    FileLink = t.FileLink,
                }).ToList(),
            }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<CaseViewModel> GetCaseNoTracking(long caseId, string currentUserId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Where(x => x.ID == caseId).Select(x => new CaseViewModel()
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
                Files = x.Files.Select(t => new CaseFileViewModel()
                {
                    ID = t.ID,
                    FileName = t.FileName,
                    FileType = t.FileType,
                    FileLink = t.FileLink,
                }).ToList(),
                LikedByCurrentUser = x.Likes.Any(y => y.LikedByUserId == currentUserId),
            }).FirstOrDefaultAsync();
            return result;
        }

        [Authorize]
        public async Task<Result> AddNewCase(AddEditCaseViewModel inputCase)
        {

            using var _context = DbFactory.CreateDbContext();

            var newCase = new Case();

            newCase.UserId = inputCase.UserId;
            newCase.Status = true;
            newCase.DateLastUpdated = DateTime.Now;
            newCase.DateCreated = DateTime.Now;
            newCase.Type = inputCase.Type;
            newCase.Topic = inputCase.Topic;
            newCase.Description = inputCase.Description;
            newCase.PatientAge = inputCase.PatientAge;
            newCase.Gender = inputCase.Gender;
            newCase.Race = inputCase.Race;
            newCase.Ethnicity = inputCase.Ethnicity;
            newCase.LabValues = inputCase.LabValues;
            newCase.CurrentStageOfDisease = inputCase.CurrentStageOfDisease;
            newCase.PostedAnonymosley = inputCase.PostAnonymosly;
            newCase.CurrentTreatmentAdministered = inputCase.CurrentTreatmentAdministered;
            newCase.TreatmentOutcomes = inputCase.TreatmentOutcomes;
            if (inputCase.SelectedTagsObjects != null && inputCase.SelectedTagsObjects.Any())
            {
                newCase.Tags = new List<Tag>();
                foreach (var tag in inputCase.SelectedTagsObjects)
                {
                    var foundTag = _context.Tags.Find(tag.ID);
                    if (foundTag != null)
                    {
                        newCase.Tags.Add(foundTag);
                    }
                }
            }

            try
            {
                await _context.AddAsync(newCase);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var tem = e;
                return Result.Failure(new List<string>() { "Error saving case." });
            }

            return Result.Success(newCase.ID);
        }

        [Authorize]
        public async Task<Result> EditCase(AddEditCaseViewModel inputCase)
        {
            using var _context = DbFactory.CreateDbContext();

            var caseObject = await _context.Cases.Include(x => x.Tags).FirstOrDefaultAsync(m => m.ID == inputCase.ID);

            if (caseObject == null)
            {
                return Result.Failure(new List<string>() { "Case not found." });
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
                caseObject.Tags = new List<Tag>();
                foreach (var tag in inputCase.SelectedTagsObjects)
                {
                    var foundTag = _context.Tags.Find(tag.ID);
                    if (foundTag != null)
                    {
                        caseObject.Tags.Add(foundTag);
                    }
                }
            }

            try
            {
                _context.Cases.Update(caseObject);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //if (!CaseExists(inputCase.ID))
                //{
                //    return false;
                //}
                //else
                //{
                //    throw;
                //}

                var t = e;
                return Result.Failure(new List<string>() { "Error saving case." });
            }

            return Result.Success(caseObject.ID);
        }

        private bool CaseExists(long id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Any(e => e.ID == id);
        }

        public async Task<bool> CheckLike(string userId, long caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseLikes.AnyAsync(v => v.LikedByUserId == userId && v.CaseId == caseId);
        }

        public async Task<bool> AddLike(string userId, long caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.CaseLikes.Add(new CaseLike()
            {
                CaseId = caseId,
                LikedByUserId = userId,
                LikeDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RemoveLike(string userId, long caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            var like = await _context.CaseLikes.FirstOrDefaultAsync(v => v.LikedByUserId == userId && v.CaseId == caseId);
            if (like != null)
            {
                _context.CaseLikes.Remove(like);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Result> SaveCaseFile(Stream fileStream, long caseId, string fileName, string fileType, string userId)
        {
            try
            {
                using var _context = DbFactory.CreateDbContext();
                var extension = Path.GetExtension(fileName);
                var plainFileName = Path.GetFileNameWithoutExtension(fileName);
                var uniquefileName = plainFileName + Guid.NewGuid().ToString() + extension;
                // await _azureStorage.SetBlobFile(uniquefileName, fileStream).ConfigureAwait(true);
                await _fileClient.SaveFile("casefiles", uniquefileName, fileStream);

                var fileLink = await _fileClient.GetFileUrl("casefiles", uniquefileName);
                var newCaseFile = new CaseFile();
                newCaseFile.CaseID = caseId;
                newCaseFile.FileName = uniquefileName;
                newCaseFile.FileType = fileType;
                newCaseFile.FileLink = fileLink;
                newCaseFile.UploadDate = DateTime.Now;
                newCaseFile.UploadedByUserId = userId;
                _context.CaseFiles.Add(newCaseFile);
                await _context.SaveChangesAsync();
                return Result.Success(newCaseFile.ID);
            }
            catch (Exception e)
            {
                var t = e;
                return Result.Failure(new List<string>() { "Error saving case comment file" });
            }
        }

        public async Task<FileDownloadResult> DownloadCaseFile(long id)
        {
            using var _context = DbFactory.CreateDbContext();
            var file = await _context.CaseFiles.FirstOrDefaultAsync(x => x.ID == id);
            var fileExists = await _fileClient.FileExists("casefiles", file.FileName);

            var result = new FileDownloadResult();
            if (file != null && fileExists)
            {
                result.FileName = file.FileName;
                result.FileType = file.FileType;
                result.FileContent = await _fileClient.GetFile("casefiles", file.FileName);
                return result;
            }
            else
                return null;
        }


        //comments
        public async Task<List<CaseCommentViewModel>> GetCommentsNoTracking(long caseId, string currentUserId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.CaseComments.Include(x=>x.CaseCommentVotes).Where(c => c.CaseID == caseId).Select(x =>
              new CaseCommentViewModel()
              {
                  CaseID = x.CaseID,
                  Comment = x.Comment,
                  ID = x.ID,
                  PostedDate = x.PostedDate,
                  Files = x.Files,
                  PosterName = x.User.FullName,
                  PosterId = x.User.Id,
                  PosterSpeciality = @EnumsHelper.GetDisplayName(x.User.MedicalSpecialty),
                  PosterImage = x.User.ProfileUrl,
                  IsAnonymos = x.IsAnonymus,
                  IsMyOwnComment = x.UserId == currentUserId,
                  VotedByCurrentUser=x.CaseCommentVotes.Any(x => x.UserId == currentUserId),
              }).AsNoTracking().ToListAsync();


            //.Include(x => x.User).Include(x => x.Files).AsNoTracking().ToListAsync();
            return result;
        }



        public async Task<int> GetCommentAddedCount(string posterId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseComments.AsNoTracking().CountAsync(c => c.UserId == posterId);
        }

        public async Task<CaseCommentViewModel> GetComment(long commentId, string currentUserId)
        {
            using var _context = DbFactory.CreateDbContext();
            var result = await _context.CaseComments.Where(x => x.ID == commentId).Select(x =>
             new CaseCommentViewModel()
             {
                 CaseID = x.CaseID,
                 Comment = x.Comment,
                 ID = x.ID,
                 PostedDate = x.PostedDate,
                 Files = x.Files,
                 PosterName = x.User.FullName,
                 PosterId = x.User.Id,
                 PosterSpeciality = @EnumsHelper.GetDisplayName(x.User.MedicalSpecialty),
                 PosterImage = x.User.ProfileUrl,
                 IsAnonymos = x.IsAnonymus,
                 IsMyOwnComment = x.UserId == currentUserId,
             }).FirstOrDefaultAsync();
            return result;
        }

        [Authorize]
        public async Task<Result> AddComment(long caseId, string comment, string userId)
        {
            using var _context = DbFactory.CreateDbContext();
            var addComment = new CaseComment();
            addComment.PostedDate = DateTime.Now;
            addComment.LastUpdatedDate = DateTime.Now;
            addComment.CaseID = caseId;
            addComment.Comment = comment;
            addComment.UserId = userId;
            _context.CaseComments.Add(addComment);
            await _context.SaveChangesAsync();


            return Result.Success(addComment.ID);
        }

        [Authorize]
        public async Task<Result> UpdateComment(long commentId, string comment)
        {
            using var _context = DbFactory.CreateDbContext();
            var caseComment = await _context.CaseComments.FirstOrDefaultAsync(m => m.ID == commentId);

            if (caseComment == null)
            {
                return Result.Failure(new List<string>() { "The comment is not found." });
            }

            caseComment.LastUpdatedDate = DateTime.Now;
            caseComment.Comment = comment;
            _context.CaseComments.Update(caseComment);
            await _context.SaveChangesAsync();


            return Result.Success(caseComment.ID);
        }

        public async Task<Result> SaveCaseCommentFile(Stream fileStream, long caseCommentId, string fileName, string fileType, string userId)
        {
            try
            {
                using var _context = DbFactory.CreateDbContext();
                var extension = Path.GetExtension(fileName);
                var plainFileName = Path.GetFileNameWithoutExtension(fileName);
                var uniquefileName = plainFileName + Guid.NewGuid().ToString() + extension;
                await _azureStorage.SetBlobFile(uniquefileName, fileStream).ConfigureAwait(true);

                var fileLink = GetImgStorageUrl() + uniquefileName;
                var newCaseCommentFile = new CaseCommentFile();
                newCaseCommentFile.CaseCommentId = caseCommentId;
                newCaseCommentFile.FileName = fileName;
                newCaseCommentFile.FileType = fileType;
                newCaseCommentFile.FileLink = fileLink;
                newCaseCommentFile.UploadDate = DateTime.Now;
                newCaseCommentFile.UploadedByUserId = userId;
                _context.CaseCommentFiles.Add(newCaseCommentFile);
                await _context.SaveChangesAsync();
                return Result.Success(newCaseCommentFile.ID);
            }
            catch (Exception e)
            {
                var t = e;
                return Result.Failure(new List<string>() { "Error saving case comment file" });
            }
        }

        public async Task<Result> DeleteCommentFile(long commentFileId, string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var caseCommentFile = await _context.CaseCommentFiles.FindAsync(commentFileId);

            if (caseCommentFile != null && caseCommentFile.UploadedByUserId == userId)
            {
                _context.CaseCommentFiles.Remove(caseCommentFile);
                await _context.SaveChangesAsync();
                return Result.Success(caseCommentFile.ID);
            }
            return Result.Failure(new List<string>() { "Error deleting case comment file" }); ;
        }


        public async Task<bool> DeleteComment(long commentId, string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var caseComment = await _context.CaseComments.FindAsync(commentId);

            if (caseComment != null && caseComment.UserId == userId)
            {
                _context.CaseComments.Remove(caseComment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public string GetImgStorageUrl()
        {
            return AppSettings.StorageUrl;
        }

        public async Task<bool> CheckVote(string voterId, long commentId)
        {
            using var _context = DbFactory.CreateDbContext();

            var vote = await _context.CaseCommentVotes.FirstOrDefaultAsync(v => v.UserId == voterId && v.CaseCommentId == commentId);
            if (vote != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddVote(string voterId, long commentId)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.CaseCommentVotes.Add(new CaseCommentVote()
            {
                CaseCommentId = commentId,
                UserId = voterId,
                VoteDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RemoveVote(string voterId, long commentId)
        {
            using var _context = DbFactory.CreateDbContext();

            var vote = await _context.CaseCommentVotes.FirstOrDefaultAsync(v => v.UserId == voterId && v.CaseCommentId == commentId);
            if (vote != null)
            {
                _context.CaseCommentVotes.Remove(vote);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ReportComment(string userId, long commentId, long caseId, string reportText)
        {
            using var _context = DbFactory.CreateDbContext();

            _context.CaseCommentReports.Add(new CaseCommentReport()
            {
                CommentId = commentId,
                UserId = userId,
                DateAdded = DateTime.UtcNow,
                CaseId = caseId,
                ReportText = reportText
            });
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
