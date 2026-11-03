using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class LabRepository : GenericRepository<Lab>, ILabRepository
    {
        private readonly AppDbContext _context;
        public LabRepository(AppDbContext context)
           : base(context)
        {
            _context = context;
        }
    }
}
