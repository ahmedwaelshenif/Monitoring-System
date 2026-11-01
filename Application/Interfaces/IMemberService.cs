using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IMemberService
    {
        Task CreateAsync(Member member);
        Task<Member?> GetByIdAsync(int id);
    }
}
