using Application.Common;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class WarningRepository : GenericRepository<Warning>, IWarningRepository
    {
        private readonly AppDbContext _context;

        public WarningRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<Warning>> GetAllWithDetailsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Warnings
                        .AsNoTracking()
                        .Include(e => e.Lab);
                        

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Warning>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<Warning>> GetWarningsByAlertAsync(Alert warning, int pageNumber, int pageSize)
        {
            var query = _context.Warnings
            .AsNoTracking()
            .Where(I => I.warning==warning)
            .Include(e => e.Lab);


            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Warning>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<Warning>> GetWarningsByLabAsync(int LabID, int pageNumber, int pageSize)
        {
                var query = _context.Warnings
            .AsNoTracking()
            .Where(I => I.LabId == LabID)
            .Include(e => e.Lab);


            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Warning>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }

}
