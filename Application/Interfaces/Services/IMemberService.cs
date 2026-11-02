using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IMemberService
    {
        Task CreateAsync(Member member);
        Task<Member?> GetByIdAsync(int id);
        Task<PagedResponse<MemberResponseDto>> GetAllAsync(int pageNumber, int pageSize);
    }
}
