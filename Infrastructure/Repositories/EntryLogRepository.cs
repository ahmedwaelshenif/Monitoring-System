using Application.Common;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EntryLogRepository : GenericRepository<EntryLog>, IEntryLogRepository
    {
        private readonly AppDbContext _context;

        public EntryLogRepository(AppDbContext context)
           : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<EntryLog>> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
        {
            var query = _context.EntryLogs
                .AsNoTracking()
                .Include(e => e.Lab)
                .Include(e => e.Member);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.EventTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EntryLog>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}
