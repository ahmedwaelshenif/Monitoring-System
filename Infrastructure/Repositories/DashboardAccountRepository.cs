using Domain.Entities;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class DashboardAccountRepository : GenericRepository<DashboardAccount>, IDashboardAccountRepository
    {
        private readonly AppDbContext _context;

        public DashboardAccountRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
