using Hippra.Data;
using Hippra.Models.POCO;
using Hippra.Models.SQL;
using Hippra.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Services
{
    public interface IHistoryLogService
    {
        Task<int> AddHistory(PostHistory newHistory);
        Task<HistoryResultModel> GetPostHistories(int posterID, int targetPage, int PageSize);

        Task<HistoryResultModel> GetPostHistories(int posterID);

        Task<PostHistory> GetHistoryByIDs(int id);

        Task AddToHistory(string historyType, AddEditCaseViewModel caseItem);

        Task<int> addToHistory(string historyType, AppUser user);

        Task addToHistoryComment(string historyType, Case c);
    }
}
