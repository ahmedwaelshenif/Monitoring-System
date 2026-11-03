using Application.Common;
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
    }
}
