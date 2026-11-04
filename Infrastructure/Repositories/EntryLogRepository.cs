using Application.Common;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
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



        public async Task<PagedResult<EntryLog>> GetPagedLogsByLabAsync(int LabId,int pageNumber, int pageSize)
        {
            var query = _context.EntryLogs
                        .AsNoTracking()
                        .Where(I => I.LabId==LabId)
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

        public async Task<PagedResult<EntryLog>> GetPagedAcceptedTodayAsync(int pageNumber, int pageSize)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var query = _context.EntryLogs
                .AsNoTracking()
                .Where(l => l.status == EntryStatus.Accepted &&
                            l.EventTime >= today && l.EventTime < tomorrow)
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

        public async Task<PagedResult<EntryLog>> GetPagedRejectedTodayAsync(int pageNumber, int pageSize)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var query = _context.EntryLogs
                .AsNoTracking()
                .Where(l => l.status == EntryStatus.Rejected &&
                            l.EventTime >= today && l.EventTime < tomorrow)
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
