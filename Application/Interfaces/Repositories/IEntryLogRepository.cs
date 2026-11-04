using Application.Common;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IEntryLogRepository : IGenericRepository<EntryLog>
    {
        Task<PagedResult<EntryLog>> GetPagedWithDetailsAsync(int pageNumber, int pageSize);
        Task<PagedResult<EntryLog>> GetPagedLogsByLabAsync(int LabId,int pageNumber, int pageSize);

        Task<PagedResult<EntryLog>> GetPagedAcceptedTodayAsync(int pageNumber, int pageSize);
        Task<PagedResult<EntryLog>> GetPagedRejectedTodayAsync(int pageNumber, int pageSize);
    }
}
