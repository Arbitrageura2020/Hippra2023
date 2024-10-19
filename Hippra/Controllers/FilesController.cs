using Hippra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace Hippra.Controllers
{
    [Route("/Files")]
    public class FilesController : Controller
    {
        private ICaseService _caseService;
        public FilesController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        [Route("/Files/Download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var fileStream = await _caseService.DownloadCaseFile(id);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileStream.FileName,
                Inline = false,
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());
            if (fileStream != null)
            {
                return File(fileStream.FileContent,fileStream.FileType);
            }
            else
                return NotFound();
        }

        [Route("/Files/DownloadCommentFile/{id}")]
        public async Task<IActionResult> DownloadCommentFile(int id)
        {
            var fileStream = await _caseService.DownloadCaseCommentFile(id);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileStream.FileName,
                Inline = false,
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());
            if (fileStream != null)
            {
                return File(fileStream.FileContent, fileStream.FileType);
            }
            else
                return NotFound();
        }
    }
}
