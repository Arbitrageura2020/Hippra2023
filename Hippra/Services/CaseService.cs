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
using Hippra.Models.DTO;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.CodeAnalysis.Operations;

namespace Hippra.Services
{
    public class CaseService : ICaseService
    {
        private IDbContextFactory<ApplicationDbContext> DbFactory;
        private AzureStorage _azureStorage;
        private AppSettings AppSettings { get; set; }
        public CaseService(
            IDbContextFactory<ApplicationDbContext> dbFactory, AzureStorage azureStorage, IOptions<AppSettings> settings)
        {
            DbFactory = dbFactory;
            _azureStorage = azureStorage;
            AppSettings = settings?.Value;
        }

        public async Task<IList<CaseViewModel>> GetMyCases(string userId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.Cases.Where(x => x.UserId == userId).Select(x => new CaseViewModel()
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
                    Name = t.Tag.Name,
                    TagId = t.TagId,
                }).ToList()
            }).AsNoTracking().ToListAsync();
            return result;
        }

        public async Task<CaseViewModel> GetCaseNoTracking(int caseId, string currentUserId)
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
                PosterSpeciality = @EnumsHelper.GetDisplayName(x.User.MedicalSpecialty),
                PosterImg = x.User.ProfileUrl,
                CurrentStageOfDisease = x.CurrentStageOfDisease,
                CurrentTreatmentAdministered = x.CurrentTreatmentAdministered,
                DateCreated = x.DateCreated,
                LabValues = x.LabValues,
                Ethnicity = x.Ethnicity,
                Gender = x.Gender,
                Type = x.Type,
                MedicalCategory = x.MedicalCategory,
                MedicalSubCategory = x.MedicalSubCategory,
                Race = x.Race,
                PatientAge = x.PatientAge,
                TreatmentOutcomes = x.TreatmentOutcomes,
                Status = x.Status,
                Tags = x.Tags.Select(t => new CaseTagViewModel()
                {
                    Name = t.Tag.Name,
                    TagId = t.TagId,
                }).ToList(),
                LikedByCurrentUser = x.Likes.Any(y => y.LikedByUserId == currentUserId),
            }).FirstOrDefaultAsync();
            return result;
        }

        [Authorize]
        public async Task<bool> AddNewCase(AddEditCaseViewModel inputCase)
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
            if (inputCase.SelectedTags != null)
            {
                newCase.Tags = new List<PostTags>();
                foreach (var tagId in inputCase.SelectedTags)
                {
                    newCase.Tags.Add(new PostTags()
                    {
                        TagId = tagId,
                        CaseID = newCase.ID,
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

        private bool CaseExists(int id)
        {
            using var _context = DbFactory.CreateDbContext();

            return _context.Cases.Include(x => x.MedicalSubCategory).AsNoTracking().Any(e => e.ID == id);
        }

        public async Task<bool> CheckLike(string userId, int caseId)
        {
            using var _context = DbFactory.CreateDbContext();

            return await _context.CaseLikes.AnyAsync(v => v.LikedByUserId == userId && v.CaseId == caseId);
        }

        public async Task<bool> AddLike(string userId, int caseId)
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


        public async Task<bool> RemoveLike(string userId, int caseId)
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


        //comments
        public async Task<List<CaseCommentViewModel>> GetCommentsNoTracking(int caseId, string currentUserId)
        {
            using var _context = DbFactory.CreateDbContext();

            var result = await _context.CaseComments.Where(c => c.CaseID == caseId).Select(x =>
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
                  IsMyOwnComment = x.UserId == currentUserId
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
                 IsMyOwnComment = x.UserId == currentUserId
             }).FirstOrDefaultAsync();
            return result;
        }

        [Authorize]
        public async Task<Result> AddComment(int caseId, string comment, string userId)
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

            var vote = await _context.CaseCommentVotes.FirstOrDefaultAsync(v => v.UserId == voterId && v.CommentId == commentId);
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
                CommentId = commentId,
                UserId = voterId,
                VoteDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RemoveVote(string voterId, long commentId)
        {
            using var _context = DbFactory.CreateDbContext();

            var vote = await _context.CaseCommentVotes.FirstOrDefaultAsync(v => v.UserId == voterId && v.CommentId == commentId);
            if (vote != null)
            {
                _context.CaseCommentVotes.Remove(vote);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
