using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        private readonly AppDbContext _context;
        public MemberRepository(AppDbContext context)
           : base(context)
        {
            _context = context;
        }
    }
}
