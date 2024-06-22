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

        public async Task<Result> SaveCaseCommentFile(Stream fileStream, long caseCommentId, string fileName, string fileType, string userId)
        {
            try
            {
                using var _context = DbFactory.CreateDbContext();
                var extension = Path.GetExtension(fileName);
                var plainFileName=Path.GetFileNameWithoutExtension(fileName);
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

        public string GetImgStorageUrl()
        {
            return AppSettings.StorageUrl;
        }

    }
}
