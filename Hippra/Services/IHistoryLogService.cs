using Hippra.Data;
using Hippra.Models.DTO;
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
        Task<long> AddHistory(AddHistoryLogDto newHistory);
        Task<HistoryResultModel> GetPostHistories(string posterID, int targetPage, int PageSize);

        Task<HistoryResultModel> GetPostHistories(string posterID);

        Task<HistoryLog> GetHistoryByIDs(int id);

        Task AddToHistory(string historyType, AddEditCaseViewModel caseItem);


        Task addToHistoryComment(string historyType, Case c);
    }
}
